using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class AddJob : Endpoint<AddJob.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/jobs");
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

        var job = new Job {
            WageRate = req.WageRate,
            JobCategory = req.JobCategory,
            JobExperience = req.JobExperience,
            JobGender = req.JobGender,
            JobType = req.JobType,
            Locale = req.Locale,
            Description = req.Description
        };

        if (req.JobType == JobType.PermanentHire) {
            job.PermanentJobDetails = new PermanentJob {
                TrialPeriod = req.TrialPeriod ?? 0
            };
        }

        // Add job to the primary worker's jobs
        job.WorkerDetails.Add(user.WorkerDetails!);

        // If it's a couple job, add it to the couple's jobs as well
        if (req.JobGender == JobGender.Couple && user.Couple != null) {
            var couple = await DbContext.Users
                .Include(u => u.WorkerDetails)
                .FirstAsync(u => u.UserId == user.CoupleUserId);
            job.WorkerDetails.Add(couple.WorkerDetails!);
        }

        await DbContext.Jobs.AddAsync(job, ct);
        await DbContext.SaveChangesAsync(ct);

        await SendResultAsync(Results.Ok(new {
            job.JobId,
            job.WageRate,
            job.JobCategory,
            job.JobExperience,
            job.JobGender,
            job.JobType,
            job.Locale,
            job.Description,
            TrialPeriod = job.PermanentJobDetails?.TrialPeriod
        }));
    }

    public record struct Request(
        int WageRate,
        JobCategory JobCategory,
        JobExperience JobExperience,
        JobGender JobGender,
        JobType JobType,
        string Locale,
        string Description,
        int? TrialPeriod);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.WageRate).NotEmpty().GreaterThan(0);

            RuleFor(r => r.JobCategory).NotEmpty().IsInEnum();
            RuleFor(r => r.JobExperience).NotEmpty().IsInEnum();
            RuleFor(r => r.JobGender).NotEmpty().IsInEnum();
            RuleFor(r => r.JobType).NotEmpty().IsInEnum();
            RuleFor(r => r.Locale).NotEmpty();
            RuleFor(r => r.Description).NotEmpty().MaximumLength(1000);
            
            When(r => r.JobType == JobType.PermanentHire, () => {
                RuleFor(r => r.TrialPeriod)
                    .NotNull()
                    .WithMessage("Trial period is required for permanent jobs")
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Trial period must be non-negative");
            });
        }
    }
}