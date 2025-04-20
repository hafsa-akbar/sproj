using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Jobs;

public class GetWorkerDetails : Endpoint<GetWorkerDetails.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/jobs/{JobId}");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var job = await DbContext.Jobs
            .Include(j => j.WorkerDetails)
                .ThenInclude(w => w.User)
            .Include(j => j.WorkerDetails.Jobs)
            .Include(j => j.WorkerDetails.PastJobs)
            .AsSplitQuery()
            .FirstOrDefaultAsync(j => j.JobId == req.JobId);


        if (job == null) {
            await SendNotFoundAsync();
            return;
        }

        var worker = job.WorkerDetails;
        if (worker == null || worker.User == null) {
            await SendNotFoundAsync();
            return;
        }

        await SendResultAsync(Results.Ok(new {
            worker = new {
                worker.User.FullName,
                worker.User.PhoneNumber,
                worker.User.Address,
                worker.User.Gender,
                worker.User.CnicNumber,
                worker.User.DrivingLicense,
                isCouple = worker.User.CoupleUserId != null
            },
            activeJobs = worker.Jobs?.Select(j => new {
                j.JobId,
                j.WageRate,
                j.JobCategory,
                j.JobExperience,
                j.JobGender,
                j.JobType,
                j.Locale
            }).ToList(),
            pastJobs = worker.PastJobs?.Select(p => new {
                p.PastJobId,
                p.JobCategory,
                p.JobGender,
                p.JobType,
                p.Locale,
                p.IsVerified,
                p.Rating,
                p.Comments
            }).ToList()
        }));
    }

    public record struct Request(int JobId);
} 