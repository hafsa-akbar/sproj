using FastEndpoints;
using sproj.Data;
using Microsoft.EntityFrameworkCore;

namespace sproj.Features.Jobs;

public class GetJobsEndpoint : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var jobs = await DbContext.Jobs
            .Include(j => j.JobCategory)
            .Include(j => j.JobType)
            .Include(j => j.JobExperience)
            .Include(j => j.Locale)
            .Select(j => new {
                j.JobId,
                j.WageRate,
                j.UserId,
                JobCategory = j.JobCategory!.JobCategoryDescription,
                JobType = j.JobType!.JobTypeDescription,
                JobExperience = j.JobExperience!.JobExperienceDescription,
                Locale = j.Locale!.LocaleName,
                IsPermanent = j.PermanentJobDetails != null
            })
            .ToListAsync(ct);

        await SendResultAsync(Results.Ok(jobs));
    }
}
