using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetJobDetails : Endpoint<GetJobDetails.Request, GetJobDetails.Response>
{
    public required AppDbContext DbContext { get; set; }

    public override void Configure()
    {
        Get("/jobs/{JobId}");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var job = await DbContext.Jobs
            .AsSplitQuery()
            .Include(j => j.WorkerDetails)
              .ThenInclude(w => w.User)
            .Include(j => j.PermanentJobDetails)
            .FirstOrDefaultAsync(j => j.JobId == req.JobId, ct);

        if (job == null)
        {
            await SendNotFoundAsync();
            return;
        }

        // Current job's workers
        var workerInfos = job.WorkerDetails
            .Select(w => new Response.WorkerInfo(
                w.User!.FullName,
                w.User.PhoneNumber,
                w.User.Gender
            ))
            .ToList();

        // Gather past jobs
        var workerIds = job.WorkerDetails.Select(w => w.UserId).ToList();
        var pastJobEntities = await DbContext.PastJobs
            .AsSplitQuery()
            .Include(p => p.WorkerDetails)
              .ThenInclude(w => w.User)
            .Where(p => p.WorkerDetails.Any(w => workerIds.Contains(w.UserId)))
            .OrderByDescending(p => p.JobCategory == job.JobCategory)
            .ThenByDescending(p => p.IsVerified)
            .ThenByDescending(p => p.PastJobId)
            .ToListAsync(ct);

        var pastJobs = pastJobEntities.Select(p =>
        {
            // Lookup employer name
            var employer = DbContext.Users
                .FirstOrDefault(u => u.PhoneNumber == p.EmployerPhoneNumber);
            var employerName = employer?.FullName ?? p.EmployerPhoneNumber;

            // Past job's workers
            var pastWorkerInfos = p.WorkerDetails
                .Select(w => new Response.WorkerInfo(
                    w.User!.FullName,
                    w.User.PhoneNumber,
                    w.User.Gender
                ))
                .ToList();

            return new Response.PastJob(
                p.PastJobId,
                p.JobCategory,
                p.JobGender,
                p.JobType,
                p.Locale,
                p.Description,
                employerName,
                p.IsVerified,
                p.Rating,
                p.Comments,
                pastWorkerInfos
            );
        }).ToList();

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
            workerInfos,
            pastJobs
        )));
    }

    public class Request
    {
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
        List<Response.WorkerInfo> Workers,       
        List<Response.PastJob> PastJobs 
    )
    {
        public record WorkerInfo(
            string FullName,
            string PhoneNumber,
            UserGender Gender
        );

        public record PastJob(
            int PastJobId,
            JobCategory JobCategory,
            JobGender JobGender,
            JobType JobType,
            string Locale,
            string Description,
            string EmployerName,
            bool IsVerified,
            int? Rating,
            string? Comments,
            List<Response.WorkerInfo> Workers
        );
    }
}
