using FastEndpoints;
using FluentValidation;
using sproj.Authentication;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;
using Microsoft.Extensions.Logging;

namespace sproj.Features.Registration;

public class Login : Endpoint<Login.Request, Login.UserResponse> {
    public required AppDbContext DbContext { get; set; }
    public required SessionStore SessionStore { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }

    public override void Configure() {
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var normalizedPhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber);

        var user = await DbContext.Users
            .Include(u => u.Couple)
            .SingleOrDefaultAsync(u => u.PhoneNumber == normalizedPhoneNumber, ct);

        if (user is null) {
            Logger.LogWarning("Login failed: User is null");
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (!PasswordHasher.VerifyHashedPassword(user.Password, req.Password)) {
            Logger.LogWarning("Password hasher failed");
            await SendUnauthorizedAsync(ct);
            return;
        }

        var sessionId = SessionStore.CreateSession(user);
        HttpContext.Response.Cookies.Append("session", sessionId, new CookieOptions {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });

        var response = new UserResponse(
            UserId: user.UserId,
            PhoneNumber: user.PhoneNumber,
            FullName: user.FullName,
            Address: user.Address,
            Birthdate: user.Birthdate,
            Gender: user.Gender,
            Role: user.Role,
            CoupleName: user.Couple?.FullName
        );

        await SendOkAsync(response, ct);
    }

    public record struct Request(string PhoneNumber, string Password);

    public record UserResponse(
        int UserId,
        string PhoneNumber,
        string FullName,
        string Address,
        DateOnly Birthdate,
        UserGender Gender,
        Role Role,
        string? CoupleName
    );

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}

// TODO: Add proper error handling with reasons - 401 when password incorrect