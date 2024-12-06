using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetJobs : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var jobs = await DbContext.Jobs.ToListAsync();

        await SendResultAsync(Results.Ok(jobs.Select(j => new {
            j.JobId,
            j.WageRate,
            j.JobCategory,
            j.JobExperience,
            j.JobGender,
            j.JobType,
            j.Locale
        })));
    }
}
