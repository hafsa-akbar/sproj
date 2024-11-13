using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

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
                j.JobCategory,
                j.JobType,
                j.JobExperience,
                j.Locale,
                IsPermanent = j.PermanentJobDetails != null
            })
            .ToListAsync(ct);

        await SendResultAsync(Results.Ok(jobs));
    }
}
