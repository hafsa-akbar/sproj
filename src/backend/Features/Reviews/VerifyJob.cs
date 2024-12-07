using FastEndpoints;
using Microsoft.EntityFrameworkCore;
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

        var pendingJob = DbContext.PastJobs
            .Where(p => !p.IsVerified)
            .Where(p => p.EmployerPhoneNumber == user.PhoneNumber)
            .Where(p => p.PastJobId == req.PastJobId)
            .FirstOrDefault();

        if (pendingJob is null) {
            await SendResultAsync(Results.BadRequest());
            return;
        }

        pendingJob.IsVerified = true;
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok());
    }

    public record struct Request(int PastJobId);
}