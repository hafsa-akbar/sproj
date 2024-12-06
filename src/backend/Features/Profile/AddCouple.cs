using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Features.Profile;

// TODO: Add by phone number instead
// TODO: Approval system
public class AddCouple : Endpoint<AddCouple.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/profile/add-couple");
        Policy(p => p.RequireClaim("role", Role.Worker.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var userId = int.Parse(User.FindFirst("user_id")!.Value);
        var user = await DbContext.Users.FirstAsync(u => u.UserId == userId);
        var couple = await DbContext.Users.FirstOrDefaultAsync(u => u.UserId == req.Couple);

        if (couple is null)
            ThrowError("user does not exist");

        if (couple.Gender == user.Gender)
            ThrowError("gender must be different");

        user.Couple = couple;
        couple.Couple = user;
        await DbContext.SaveChangesAsync();

        await SendOkAsync();
    }

    public record Request(int Couple);
}
