using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.WorkHistory;

public class AddPastJob : Endpoint<AddPastJob.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }
    public required ISmsSender SmsSender { get; set; }

    public override void Configure() {
        Post("/past-jobs");
        Policies("Worker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.Include(u => u.Couple).Include(u => u.WorkerDetails)
            .FirstAsync(u => u.UserId == userId);

        var empPhoneNumber = Utils.NormalizePhoneNumber(req.EmployerPhoneNumber);
        if (user.PhoneNumber == empPhoneNumber) {
            await SendUnauthorizedAsync();
            return;
        }

        var job = new PastJob {
            JobCategory = req.JobCategory,
            JobGender = req.JobGender,
            JobType = req.JobType,
            Locale = req.Locale,
            EmployerPhoneNumber = empPhoneNumber,
            IsVerified = false
        };

        var employer = await DbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == empPhoneNumber);
        if (employer is null)
            SmsSender.SendMessage(empPhoneNumber,
                $"{user.FullName} has asked for a reccomendation for their work in {job.JobCategory.ToString()}. Sign up at DaamPeKaam to reccomend them.");

        user.WorkerDetails!.PastJobs!.Add(job);
        await DbContext.SaveChangesAsync();

        await SendOkAsync();
    }

    public record struct Request(
        JobCategory JobCategory,
        JobGender JobGender,
        JobType JobType,
        string Locale,
        string EmployerPhoneNumber);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.JobGender).NotEmpty().IsInEnum();
            RuleFor(r => r.JobCategory).NotEmpty().IsInEnum();
            RuleFor(r => r.JobType).NotEmpty().IsInEnum();
            RuleFor(r => r.Locale).NotEmpty();

            RuleFor(r => r.EmployerPhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber);
        }
    }
}
