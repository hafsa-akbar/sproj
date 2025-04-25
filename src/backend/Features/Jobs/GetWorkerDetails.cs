using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetWorkerDetails : Endpoint<EmptyRequest, GetWorkerDetails.Response> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/worker-details");
        Policies("Worker");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users
            .AsSplitQuery()
            .Include(u => u.WorkerDetails)
            .ThenInclude(w => w!.Jobs!.OrderByDescending(j => j.JobId))
            .ThenInclude(j => j.WorkerDetails)
            .ThenInclude(w => w.User)
            .Include(u => u.WorkerDetails)
            .ThenInclude(w => w!.PastJobs!.OrderByDescending(p => p.PastJobId))
            .ThenInclude(p => p.WorkerDetails)
            .ThenInclude(w => w.User)
            .FirstAsync(u => u.UserId == userId, ct);

        if (user.WorkerDetails == null) {
            await SendNotFoundAsync();
            return;
        }

        var workerDetails = user.WorkerDetails;
        var jobs = (workerDetails.Jobs ?? Enumerable.Empty<Job>())
            .Select(j => new Response.Job(
                j.JobId,
                j.WageRate,
                j.JobCategory,
                j.JobExperience,
                j.JobGender,
                j.JobType,
                j.Locale,
                j.Description,
                j.PermanentJobDetails?.TrialPeriod,
                j.WorkerDetails.Select(w => w.User!.FullName).ToList()
            ))
            .ToList();

        var pastJobs = (workerDetails.PastJobs ?? Enumerable.Empty<PastJob>())
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
            .ToList();

        await SendResultAsync(Results.Ok(new Response(
            workerDetails.Rating,
            workerDetails.NumberOfRatings,
            jobs,
            pastJobs
        )));
    }

    public record Response(
        double? Rating,
        int NumberOfRatings,
        List<Response.Job> Jobs,
        List<Response.PastJob> PastJobs
    ) {
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
            List<string> WorkerNames
        );

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