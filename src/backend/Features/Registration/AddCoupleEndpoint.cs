using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using sproj.Data;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

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

        if (couple is null) {
            var error = new ErrorResponse([new ValidationFailure("couple", "user does not exist")]);
            await error.ExecuteAsync(HttpContext);
            return;
        }

        if (couple.Gender == user.Gender) {
            var error = new ErrorResponse([new ValidationFailure("couple", "gender must be different")]);
            await error.ExecuteAsync(HttpContext);
            return;
        }

        user.Couple = couple;
        couple.Couple = user;
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok( new {
            Status = "success"
        }));
    }

    public record Request(int Couple);
}
