using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Data.Entities;
using sproj.Services;

// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class RegisterEndpoint : Endpoint<RegisterEndpoint.Request, EmptyResponse> {
    public override void Configure() {
        Post("/users/register");
        AllowAnonymous();
    }

    public required AppDbContext DbContext { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }

    public record Request(string Username, string Password, string PhoneNumber);

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        if (await DbContext.Users.AnyAsync(u => u.Username == req.Username))
            AddError(r => r.Username, "this username is already in use");

        ThrowIfAnyErrors();

        var newUser = new User {
            Username = req.Username,
            PhoneNumber = req.PhoneNumber,
            Password = PasswordHasher.HashPassword(req.Password)
        };

        DbContext.Users.Add(newUser);
        await DbContext.SaveChangesAsync();

        await SendOkAsync();
    }
}
