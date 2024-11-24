using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Profile;

public class UpdatePreferences : Endpoint<UpdatePreferences.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Patch("/profile/preferences");
        Policy(p => p.RequireClaim("role", Role.Employer.ToString(), Role.Worker.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);

        var user = await DbContext.Users
            .Include(u => u.UserPreferences)
            .FirstAsync(u => u.UserId == userId);

        if (req.JobLocale is not null)
            user.UserPreferences!.JobLocale = req.JobLocale;

        if (req.JobCategories is not null)
            user.UserPreferences!.JobCategories = req.JobCategories.Distinct().Order().ToList();

        if (req.JobExperiences is not null)
            user.UserPreferences!.JobExperiences = req.JobExperiences.Distinct().Order().ToList();

        if (req.JobTypes is not null)
            user.UserPreferences!.JobTypes = req.JobTypes.Distinct().Order().ToList();

        await DbContext.SaveChangesAsync();
        await SendResultAsync(Results.Ok(user.UserPreferences));
    }

    public record struct Request(
        string? JobLocale,
        List<JobCategory>? JobCategories,
        List<JobType>? JobTypes,
        List<JobExperience>? JobExperiences);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.JobLocale).MaximumLength(64);
            RuleForEach(r => r.JobCategories).IsInEnum();
            RuleForEach(r => r.JobTypes).IsInEnum();
            RuleForEach(r => r.JobExperiences).IsInEnum();
        }
    }
}