using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetJobs : Endpoint<EmptyRequest, List<GetJobs.Response>> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct) {
        var jobs = await DbContext.Jobs
            .AsSplitQuery()
            .Include(j => j.WorkerDetails)
            .ThenInclude(w => w.User)
            .OrderByDescending(j => j.JobId)
            .Select(j => new Response(
                j.JobId,
                j.WageRate,
                j.JobCategory,
                j.JobExperience,
                j.JobGender,
                j.JobType,
                j.Locale,
                j.Description,
                j.PermanentJobDetails != null ? j.PermanentJobDetails.TrialPeriod : null,
                j.WorkerDetails.Select(w => w.User != null ? w.User.FullName : "Unknown").ToList()
            ))
            .ToListAsync(ct);

        await SendResultAsync(Results.Ok(jobs));
    }

    public record Response(
        int JobId,
        int WageRate,
        JobCategory JobCategory,
        JobExperience JobExperience,
        JobGender JobGender,
        JobType JobType,
        string Locale,
        string Description,
        int? TrialPeriod,
        List<string> WorkerNames
    );
}