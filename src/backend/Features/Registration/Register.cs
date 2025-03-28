using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sproj.Authentication;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Registration;

public class Register : Endpoint<Register.Request, Register.Response> {
    public required AppDbContext DbContext { get; set; }
    public required SessionStore SessionStore { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }

    public override void Configure() {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        Logger.LogInformation("Starting registration process");
        var normalizedPhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber);

        if (await DbContext.Users.AnyAsync(u => u.PhoneNumber == normalizedPhoneNumber))
            ThrowError(r => r.PhoneNumber, "phone number is already in use");

        var user = new User {
            PhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber),
            Password = PasswordHasher.HashPassword(req.Password),
            Role = Role.Unregistered,

            FullName = req.FullName,
            Address = req.Address,
            Birthdate = req.Birthdate,
            Gender = req.Gender
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        Logger.LogInformation("User created with ID: {UserId}", user.UserId);

        var sessionId = SessionStore.CreateSession(user);
        Logger.LogInformation("Session created with ID: {SessionId}", sessionId);

        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        var cookieOptions = new CookieOptions {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax
        };
        
        Logger.LogInformation("Setting cookie with options: HttpOnly={HttpOnly}, Secure={Secure}, SameSite={SameSite}", 
            cookieOptions.HttpOnly, cookieOptions.Secure, cookieOptions.SameSite);
        
        HttpContext.Response.Cookies.Append("session", sessionId, cookieOptions);
        Logger.LogInformation("Cookie set in response");

        await SendOkAsync(new Response(user.UserId, user.PhoneNumber, user.FullName, user.Role), ct);
        Logger.LogInformation("Registration completed successfully");
    }

    public record struct Request(
        string PhoneNumber,
        string Password,
        string FullName,
        string Address,
        DateOnly Birthdate,
        UserGender Gender);

    public record struct Response(
        int UserId,
        string PhoneNumber,
        string FullName,
        Role Role);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Birthdate).NotEmpty();
            RuleFor(x => x.Gender).NotEmpty().IsInEnum();
        }
    }
}