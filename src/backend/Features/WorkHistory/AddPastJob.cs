using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.WorkHistory;

public class AddPastJob : Endpoint<AddPastJob.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required ISmsSender SmsSender { get; set; }

    public override void Configure() {
        Post("/past-jobs");
        Policies("Worker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users
            .Include(u => u.Couple)
            .Include(u => u.WorkerDetails)
            .FirstAsync(u => u.UserId == userId);

        if (req.JobGender == JobGender.Couple && user.Couple == null)
            ThrowError(r => r.JobGender, "user not a couple");

        var empPhoneNumber = Utils.NormalizePhoneNumber(req.EmployerPhoneNumber);
        if (user.PhoneNumber == empPhoneNumber || user.Couple?.PhoneNumber == empPhoneNumber) {
            await SendUnauthorizedAsync();
            return;
        }

        var job = new PastJob {
            JobCategory = req.JobCategory,
            JobGender = req.JobGender,
            JobType = req.JobType,
            Locale = req.Locale,
            Description = req.Description,
            EmployerPhoneNumber = empPhoneNumber,
            IsVerified = false
        };

        // Add job to the primary worker's past jobs
        job.WorkerDetails.Add(user.WorkerDetails!);

        // If it's a couple job, add it to the couple's past jobs as well
        if (req.JobGender == JobGender.Couple && user.Couple != null) {
            var couple = await DbContext.Users
                .Include(u => u.WorkerDetails)
                .FirstAsync(u => u.UserId == user.CoupleUserId);
            job.WorkerDetails.Add(couple.WorkerDetails!);
        }

        var employer = await DbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == empPhoneNumber);
        if (employer is null)
            SmsSender.SendMessage(empPhoneNumber,
                $"{user.FullName} has asked for a recommendation for their work in {job.JobCategory.ToString()}. Sign up at DaamPeKaam to recommend them.");

        await DbContext.PastJobs.AddAsync(job, ct);
        await DbContext.SaveChangesAsync(ct);

        await SendResultAsync(Results.Ok(new {
            job.PastJobId,
            job.JobCategory,
            job.JobGender,
            job.JobType,
            job.Locale,
            job.Description,
            job.EmployerPhoneNumber,
            job.IsVerified
        }));
    }

    public record struct Request(
        JobCategory JobCategory,
        JobGender JobGender,
        JobType JobType,
        string Locale,
        string Description,
        string EmployerPhoneNumber);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.JobGender).NotEmpty().IsInEnum();
            RuleFor(r => r.JobCategory).NotEmpty().IsInEnum();
            RuleFor(r => r.JobType).NotEmpty().IsInEnum();
            RuleFor(r => r.Locale).NotEmpty();
            RuleFor(r => r.Description).NotEmpty().MaximumLength(1000);
            RuleFor(r => r.EmployerPhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber);
        }
    }
}