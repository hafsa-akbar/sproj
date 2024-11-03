using FastEndpoints;
using sproj.Data;
using sproj.Data.Entities;
using sproj.Services;

// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class LoginEndpoint : Endpoint<LoginEndpoint.Request, LoginEndpoint.Response> {
    public override void Configure() {
        Post("/users/login");
        AllowAnonymous();
    }

    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }

    public record struct Request(string Username, string Password);

    public record struct Response(string Token);

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var dummyUser = new User {
            Password = string.Empty,
            Username = null!,
            PhoneNumber = null!
        };

        var user = DbContext.Users.SingleOrDefault(u => u.Username == req.Username) ?? dummyUser;
        if (!PasswordHasher.VerifyHashedPassword(user.Password, req.Password)) {
            await SendResultAsync(TypedResults.Unauthorized());
            return;
        }

        await SendAsync(new Response {
            Token = JwtCreator.CreateJwt(user)
        });
    }
}
