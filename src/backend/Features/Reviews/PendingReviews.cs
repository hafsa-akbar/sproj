using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Reviews;

public class PendingReviews : Endpoint<EmptyRequest, List<PendingReviews.Response>> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/pending-reviews");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        var pendingJobs = await DbContext.PastJobs
            .Include(p => p.WorkerDetails)
            .ThenInclude(w => w.User)
            .Where(p => !p.IsVerified)
            .Where(p => p.EmployerPhoneNumber == user.PhoneNumber)
            .OrderByDescending(p => p.PastJobId)
            .Select(p => new Response(
                p.PastJobId,
                p.JobCategory,
                p.JobGender,
                p.JobType,
                p.Locale,
                p.Description,
                p.WorkerDetails.Select(w => w.User!.FullName).ToList()
            ))
            .ToListAsync(ct);

        await SendResultAsync(Results.Ok(pendingJobs));
    }

    public record Response(
        int PastJobId,
        JobCategory JobCategory,
        JobGender JobGender,
        JobType JobType,
        string Locale,
        string Description,
        List<string> WorkerNames
    );
}