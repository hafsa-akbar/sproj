using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Data.Entities;
using sproj.Services;

// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class RegisterEndpoint : Endpoint<RegisterEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }
    public required PhoneNumberUtil PhoneNumberUtil { get; set; }

    public override void Configure() {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var normalizedPhoneNumber = PhoneNumberUtil.NormalizePhoneNumber(req.PhoneNumber);

        if (await DbContext.Users.AnyAsync(u => u.PhoneNumber == normalizedPhoneNumber))
            AddError(r => r.PhoneNumber, "phone number is already in use");

        ThrowIfAnyErrors();

        var user = new User {
            PhoneNumber = PhoneNumberUtil.NormalizePhoneNumber(req.PhoneNumber),
            Password = PasswordHasher.HashPassword(req.Password)
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(new {
            Status = "success",
            Data = new {
                Token = JwtCreator.CreateJwt(user)
            }
        }));
    }

    public record struct Request(string PhoneNumber, string Password);

    public class RequestValidator : Validator<Request> {
        public RequestValidator(PhoneNumberUtil phoneNumberUtil) {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(phoneNumberUtil.ValidatePhoneNumber)
                .WithMessage("phone number is invalid");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}