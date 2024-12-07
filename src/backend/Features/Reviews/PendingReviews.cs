using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Reviews;

public class PendingReviews : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/pending-jobs");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        var pendingReviews = await DbContext.PastJobs
            .Where(p => p.EmployerPhoneNumber == user.PhoneNumber && !p.IsVerified)
            .Include(p => p.WorkerDetails)
            .ThenInclude(w => w!.User)
            .ToListAsync();

        await SendResultAsync(Results.Ok(pendingReviews.Select(p => new {
            p.PastJobId,
            p.JobCategory,
            p.JobGender,
            p.JobType,
            Worker = new {
                p.WorkerDetails!.UserId,
                p.WorkerDetails!.User!.FullName,
            }
        })));
    }
}
