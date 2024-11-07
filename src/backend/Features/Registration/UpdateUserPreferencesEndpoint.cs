using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class UpdateUserPreferencesEndpoint : Endpoint<UpdateUserPreferencesEndpoint.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Put("/user/preferences");
        Policy(p => p.RequireClaim("role", ((int)Data.Roles.Employer).ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);

        var user = await DbContext.Users
            .Include(u => u.UserPreferences)
            .FirstAsync(u => u.UserId == userId);

        user.UserPreferences = new UserPreferences { UserId = userId };

        user.UserPreferences.JobLocale = req.Locale;
        user.UserPreferences.JobCategories =
            req.JobCategories.Select(jc => new JobCategory {
                JobCategoryId = jc,
                JobCategoryDescription = null!
            }).ToList();
        user.UserPreferences.JobTypes = req.JobTypes.Select(jt => new JobType {
            JobTypeId = jt,
            JobTypeDescription = null!
        }).ToList();
        user.UserPreferences.JobExperiences =
            req.JobExperiences.Select(je => new JobExperience {
                JobExperienceId = je,
                JobExperienceDescription = null!
            }).ToList();

        await DbContext.SaveChangesAsync(ct);

        await SendResultAsync(Results.Ok(user.UserPreferences));
    }

    public record struct Request(
        Locales Locale,
        List<JobCategories> JobCategories,
        List<JobTypes> JobTypes,
        List<JobExperiences> JobExperiences);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(r => r.Locale).IsInEnum().WithMessage("Locale must be a valid value.");
            RuleForEach(r => r.JobCategories).IsInEnum().WithMessage("Job category ID must be a valid number.");
            RuleForEach(r => r.JobTypes).IsInEnum().WithMessage("Job type ID must be a valid number.");
            RuleForEach(r => r.JobExperiences).IsInEnum().WithMessage("Job experience ID must be a valid number.");
        }
    }
}