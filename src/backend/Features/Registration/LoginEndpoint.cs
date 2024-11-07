using FastEndpoints;
using FluentValidation;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class LoginEndpoint : Endpoint<LoginEndpoint.Request, EmptyRequest> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }
    public required PhoneNumberUtil PhoneNumberUtil { get; set; }

    public override void Configure() {
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var normalizedPhoneNumber = PhoneNumberUtil.NormalizePhoneNumber(req.PhoneNumber);

        var dummyUser = new User {
            Password = string.Empty,
            PhoneNumber = normalizedPhoneNumber,
            RoleId = Data.Roles.Unregistered,
            FullName = null!,
            Address = null!,
            Birthdate = default
        };

        var user = DbContext.Users.SingleOrDefault(u => u.PhoneNumber == normalizedPhoneNumber) ?? dummyUser;
        if (!PasswordHasher.VerifyHashedPassword(user.Password, req.Password)) {
            await SendResultAsync(TypedResults.Unauthorized());
            return;
        }

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