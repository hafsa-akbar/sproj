using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetJobs : EndpointWithoutRequest<GetJobs.Response> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct) {
        var jobs = await DbContext.Jobs
            .AsSplitQuery()
            .Include(j => j.WorkerDetails)
                .ThenInclude(w => w.User)
            .Include(j => j.PermanentJobDetails)
            .Select(j => new Response.Job(
                j.JobId,
                j.WageRate,
                j.JobCategory,
                j.JobExperience,
                j.JobGender,
                j.JobType,
                j.Locale,
                j.Description,
                j.PermanentJobDetails!.TrialPeriod,
                j.WorkerDetails.Select(w => new Response.WorkerInfo(
                    w.User!.FullName,
                    w.User.Gender
                )).ToList()
            ))
            .ToListAsync(ct);

        await SendResultAsync(Results.Ok(new Response(jobs)));
    }

    public record Response(List<Response.Job> Jobs) {
        public record Job(
            int JobId,
            int WageRate,
            JobCategory JobCategory,
            JobExperience JobExperience,
            JobGender JobGender,
            JobType JobType,
            string Locale,
            string Description,
            int? TrialPeriod,
            List<Response.WorkerInfo> Workers
        );

        public record WorkerInfo(
            string FullName,
            UserGender Gender
        );
    }
}