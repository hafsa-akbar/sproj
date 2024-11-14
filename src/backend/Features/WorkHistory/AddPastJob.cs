using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.WorkHistory;

public class AddPastJob : Endpoint<AddPastJob.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/past-jobs");
        Policy(p => p.RequireClaim("role", Role.Worker.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.Include(u => u.Couple).Include(u => u.WorkerDetails)
            .FirstAsync(u => u.UserId == userId);

        var job = new PastJob {
            JobCategory = req.JobCategory,
            JobGender = req.JobGender,
            JobType = req.JobType,
            Locale = req.Locale,
            EmployerPhoneNumber = Utils.NormalizePhoneNumber(req.EmployerPhoneNumber),
            IsVerified = false
        };

        user.WorkerDetails!.PastJobs!.Add(job);
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(job));
    }

    public record struct Request(
        JobCategory JobCategory,
        JobGender JobGender,
        JobType JobType,
        string Locale,
        string EmployerPhoneNumber);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.JobGender).NotNull()
                .IsInEnum().WithMessage("Job gender must be a valid value.");

            RuleFor(r => r.JobCategory).NotNull()
                .IsInEnum().WithMessage("Job category must be a valid value.");

            RuleFor(r => r.JobType).NotNull()
                .IsInEnum().WithMessage("Job type must be a valid value.");

            RuleFor(r => r.Locale).NotNull();

            RuleFor(r => r.EmployerPhoneNumber).NotNull().Must(Utils.ValidatePhoneNumber)
                .WithMessage("phone number is invalid");
        }
    }
}
