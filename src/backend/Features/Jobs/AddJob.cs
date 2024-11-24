using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Jobs;

// TODO: Debug this
public class AddJob : Endpoint<AddJob.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/jobs");
        Policy(p => p.RequireClaim("role", Role.Worker.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users
            .Include(u => u.Couple)
            .Include(u => u.WorkerDetails)
            .ThenInclude(w => w!.Jobs!)
            .FirstAsync(u => u.UserId == userId);

        if (req.JobGender == JobGender.Couple && user.Couple == null)
            ThrowError(r => r.JobGender, "user not a couple");

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

        await SendResultAsync(Results.Ok(new {
            job.JobId,
            job.WageRate,
            job.JobCategory,
            job.JobExperience,
            job.JobGender,
            job.JobType,
            job.Locale
        }));
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
            RuleFor(r => r.WageRate).NotEmpty().GreaterThan(0);

            RuleFor(r => r.JobCategory).NotEmpty().IsInEnum();
            RuleFor(r => r.JobExperience).NotEmpty().IsInEnum();
            RuleFor(r => r.JobGender).NotEmpty().IsInEnum();
            RuleFor(r => r.JobType).NotEmpty().IsInEnum();
            RuleFor(r => r.Locale).NotEmpty();
        }
    }
}
