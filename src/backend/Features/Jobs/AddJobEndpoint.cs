using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Jobs;

public class AddJobEndpoint : Endpoint<AddJobEndpoint.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/jobs");
        Policy(p => p.RequireClaim("role", Role.Worker.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.Include(u => u.Couple).Include(u => u.WorkerDetails)
            .FirstAsync(u => u.UserId == userId);

        if (req.JobGender == JobGender.Couple && user.Couple == null)
            ThrowError(r => r.JobGender, "couple job not allowed");

        var job = new Job {
            WageRate = req.WageRate,
            JobCategory = req.JobCategory,
            JobExperience = req.JobExperience,
            JobGender = req.JobGender,
            JobType = req.JobType,
            Locale = req.Locale
        };

        user.WorkerDetails!.Jobs!.Add(job);
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(job));
    }

    public record struct Request(
        int WageRate,
        JobCategory JobCategory,
        JobExperience JobExperience,
        JobGender JobGender,
        JobType JobType,
        string Locale);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.WageRate).NotNull()
                .GreaterThan(0).WithMessage("Wage rate must be greater than 0.");

            RuleFor(r => r.JobGender).NotNull()
                .IsInEnum().WithMessage("Job gender must be a valid value.");

            RuleFor(r => r.JobCategory).NotNull()
                .IsInEnum().WithMessage("Job category must be a valid value.");

            RuleFor(r => r.JobType).NotNull()
                .IsInEnum().WithMessage("Job type must be a valid value.");

            RuleFor(r => r.JobExperience).NotNull()
                .IsInEnum().WithMessage("Job experience must be a valid value.");

            RuleFor(r => r.Locale).NotNull();
        }
    }
}