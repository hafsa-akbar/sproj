using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class UpdateUserPreferencesEndpoint : Endpoint<UpdateUserPreferencesEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/user/preferences");
        Policy(p => p.RequireClaim("role", Role.Employer.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        if (req.JobCategories is null) req.JobCategories = new List<JobCategory>();
        if (req.JobTypes is null) req.JobTypes = new List<JobType>();
        if (req.JobExperiences is null) req.JobExperiences = new List<JobExperience>();

        var userId = int.Parse(User.FindFirst("user_id")!.Value);

        var user = await DbContext.Users
            .Include(u => u.UserPreferences)
            .FirstAsync(u => u.UserId == userId);

        user.UserPreferences = new UserPreferences {
            UserId = userId,
            JobLocale = req.Locale,
            JobCategories = req.JobCategories,
            JobTypes = req.JobTypes,
            JobExperiences = req.JobExperiences
        };

        await DbContext.SaveChangesAsync(ct);

        await SendResultAsync(Results.Ok(user.UserPreferences));
    }

    public record struct Request(
        string? Locale,
        List<JobCategory>? JobCategories,
        List<JobType>? JobTypes,
        List<JobExperience>? JobExperiences);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleForEach(r => r.JobCategories).IsInEnum().WithMessage("Job category ID must be a valid number.");
            RuleForEach(r => r.JobTypes).IsInEnum().WithMessage("Job type ID must be a valid number.");
            RuleForEach(r => r.JobExperiences).IsInEnum().WithMessage("Job experience ID must be a valid number.");
        }
    }
}