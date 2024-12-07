using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Profile;

public class GetPreferences : Endpoint<EmptyRequest, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Get("/profile/preferences");
        Policies("EmployerOrWorker");
    }

    public override async Task HandleAsync(EmptyRequest _, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);

        var user = await DbContext.Users
            .Include(u => u.UserPreferences)
            .FirstAsync(u => u.UserId == userId);

        await DbContext.SaveChangesAsync(ct);
        await SendResultAsync(Results.Ok(user.UserPreferences));
    }
}
