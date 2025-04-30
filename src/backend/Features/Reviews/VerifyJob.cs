using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sproj.Data;

namespace sproj.Features.Reviews;

public class VerifyJob : Endpoint<VerifyJob.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/verify-review");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        var pendingJob = await DbContext.PastJobs
            .Include(p => p.WorkerDetails)
            .ThenInclude(w => w.User)
            .Where(p => !p.IsVerified)
            .Where(p => p.EmployerPhoneNumber == user.PhoneNumber)
            .Where(p => p.PastJobId == req.PastJobId)
            .FirstOrDefaultAsync(ct);

        if (pendingJob is null) {
            Logger.LogError("Pending job not found");
            await SendResultAsync(Results.BadRequest());
            return;
        }

        pendingJob.IsVerified = true;
        pendingJob.Rating = req.Rating;
        pendingJob.Comments = req.Comments;

        // Update worker's rating if a rating was provided
        if (req.Rating.HasValue) {
            foreach (var workerDetails in pendingJob.WorkerDetails) {
                var currentTotalRating = (workerDetails.Rating ?? 0) * workerDetails.NumberOfRatings;
                workerDetails.NumberOfRatings++;
                workerDetails.Rating = (currentTotalRating + req.Rating.Value) / workerDetails.NumberOfRatings;
            }
        }

        await DbContext.SaveChangesAsync(ct);
        await SendResultAsync(Results.Ok());
    }

    public record struct Request(
        int PastJobId,
        int? Rating,
        string? Comments
    );
}
