using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Profile;

public class AddCoupleEndpoint : Endpoint<AddCoupleEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }

    public override void Configure() {
        Post("/users/add-couple");
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