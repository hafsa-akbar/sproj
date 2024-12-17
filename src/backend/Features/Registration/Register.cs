using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Authentication;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Registration;

public class Register : Endpoint<Register.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required SessionStore SessionStore { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }

    public override void Configure() {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
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

        var sessionId = SessionStore.CreateSession(user);
        HttpContext.Response.Cookies.Append("session", sessionId, new CookieOptions {
            HttpOnly = true,
            Secure = true
        });

        await SendResultAsync(Results.Ok());
    }

    public record struct Request(
        string PhoneNumber,
        string Password,
        string FullName,
        string Address,
        DateOnly Birthdate,
        UserGender Gender);

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