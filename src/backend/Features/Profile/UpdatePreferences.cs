using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Profile;

public class UpdatePreferences : Endpoint<UpdatePreferences.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Patch("/preferences");
        Policy(p => p.RequireClaim("role", Role.Employer.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);

        var user = await DbContext.Users
            .Include(u => u.UserPreferences)
            .FirstAsync(u => u.UserId == userId);

        if (req.Locale is not null)
            user.UserPreferences.JobLocale = req.Locale;

        if (req.JobCategories is not null)
            user.UserPreferences.JobCategories = req.JobCategories.Distinct().ToList();

        if (req.JobExperiences is not null)
            user.UserPreferences.JobExperiences = req.JobExperiences.Distinct().ToList();

        if (req.JobTypes is not null)
            user.UserPreferences.JobTypes = req.JobTypes.Distinct().ToList();

        await DbContext.SaveChangesAsync();
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