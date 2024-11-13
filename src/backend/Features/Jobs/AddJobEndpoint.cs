using FastEndpoints;
using FluentValidation;
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
        Logger.LogInformation("I am here");
        var job = new Job {
            WageRate = req.WageRate,
            UserId = int.Parse(User.FindFirst("user_id")!.Value),
            JobCategory = req.JobCategory,
            JobType = req.JobType,
            JobExperience = req.JobExperience,
            Locale = req.Locale,
            IsCoupleJob = false
        };

        DbContext.Jobs.Add(job);
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(job));
    }

    // TODO: Deserialize as strings
    public record struct Request(
        int WageRate,
        JobCategory JobCategory,
        JobType JobType,
        JobExperience JobExperience,
        Locale Locale);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.WageRate).NotNull()
                .GreaterThan(0).WithMessage("Wage rate must be greater than 0.");

            RuleFor(r => r.JobCategory).NotNull()
                .IsInEnum().WithMessage("Job category must be a valid value.");

            RuleFor(r => r.JobType).NotNull()
                .IsInEnum().WithMessage("Job type must be a valid value.");

            RuleFor(r => r.JobExperience).NotNull()
                .IsInEnum().WithMessage("Job experience must be a valid value.");

            RuleFor(r => r.Locale).NotNull()
                .IsInEnum().WithMessage("Locale must be a valid value.");
        }
    }
}
