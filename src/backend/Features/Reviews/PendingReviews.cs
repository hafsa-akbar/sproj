using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Reviews;

public class PendingReviews : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/pending-jobs");
        Policy(p => p.RequireClaim("role", Role.Employer.ToString()));
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);

        var pendingReviews = await DbContext.PastJobs
            .Where(p => p.EmployerPhoneNumber == user.PhoneNumber && !p.IsVerified)
            .Include(p => p.WorkerDetails)
            .ToListAsync();
        
        await SendResultAsync(Results.Ok(pendingReviews));
    }
}