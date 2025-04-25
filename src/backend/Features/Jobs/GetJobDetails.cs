using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetJobDetails : Endpoint<GetJobDetails.Request, GetJobDetails.Response> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs/{JobId}");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var job = await DbContext.Jobs
            .AsSplitQuery()
            .Include(j => j.WorkerDetails)
            .ThenInclude(w => w.User)
            .FirstOrDefaultAsync(j => j.JobId == req.JobId, ct);

        if (job == null) {
            await SendNotFoundAsync();
            return;
        }

        // Get all workers associated with this job
        var workerIds = job.WorkerDetails.Select(w => w.UserId).ToList();

        // Get past jobs for all workers, sorted by category match and verification status
        var pastJobs = await DbContext.PastJobs
            .AsSplitQuery()
            .Include(p => p.WorkerDetails)
            .ThenInclude(w => w.User)
            .Where(p => p.WorkerDetails.Any(w => workerIds.Contains(w.UserId)))
            .OrderByDescending(p => p.JobCategory == job.JobCategory) // Matching category first
            .ThenByDescending(p => p.IsVerified) // Verified jobs first within each category
            .ThenByDescending(p => p.PastJobId) // Most recent first
            .Select(p => new Response.PastJob(
                p.PastJobId,
                p.JobCategory,
                p.JobGender,
                p.JobType,
                p.Locale,
                p.Description,
                p.EmployerPhoneNumber,
                p.IsVerified,
                p.Rating,
                p.Comments,
                p.WorkerDetails.Select(w => w.User!.FullName).ToList()
            ))
            .ToListAsync(ct);

        await SendResultAsync(Results.Ok(new Response(
            job.JobId,
            job.WageRate,
            job.JobCategory,
            job.JobExperience,
            job.JobGender,
            job.JobType,
            job.Locale,
            job.Description,
            job.PermanentJobDetails?.TrialPeriod,
            job.WorkerDetails.Select(w => w.User!.FullName).ToList(),
            pastJobs
        )));
    }

    public class Request {
        public int JobId { get; set; }
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
        List<string> WorkerNames,
        List<Response.PastJob> PastJobs
    ) {
        public record PastJob(
            int PastJobId,
            JobCategory JobCategory,
            JobGender JobGender,
            JobType JobType,
            string Locale,
            string Description,
            string EmployerPhoneNumber,
            bool IsVerified,
            int? Rating,
            string? Comments,
            List<string> WorkerNames
        );
    }
} 