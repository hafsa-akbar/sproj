using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class RegisterEndpoint : Endpoint<RegisterEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }

    public override void Configure() {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var normalizedPhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber);

        if (await DbContext.Users.AnyAsync(u => u.PhoneNumber == normalizedPhoneNumber))
            AddError(r => r.PhoneNumber, "phone number is already in use");

        ThrowIfAnyErrors();

        var user = new User {
            PhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber),
            Password = Utils.HashPassword(req.Password),
            Role = Role.Unregistered,
            FullName = req.FullName,
            Address = req.Address,
            Birthdate = req.Birthdate,
            Gender = req.Gender
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

    public record struct Request(
        string PhoneNumber,
        string Password,
        string FullName,
        string Address,
        DateOnly Birthdate,
        UserGender Gender);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber)
                .WithMessage("phone number is invalid");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(128);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(512);
            RuleFor(x => x.Gender).NotEmpty().IsInEnum();
        }
    }
}