using FastEndpoints;
using FluentValidation;
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
    public required PhoneNumberUtil PhoneNumberUtil { get; set;  }


    public record struct Request(string PhoneNumber, string Password);

    public record struct Response(string Token);

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var normalizedPhoneNumber = PhoneNumberUtil.NormalizePhoneNumber(req.PhoneNumber);

        var dummyUser = new User {
            Password = string.Empty,
            PhoneNumber = normalizedPhoneNumber,
        };

        var user = DbContext.Users.SingleOrDefault(u => u.PhoneNumber == normalizedPhoneNumber) ?? dummyUser;
        if (!PasswordHasher.VerifyHashedPassword(user.Password, req.Password)) {
            await SendResultAsync(TypedResults.Unauthorized());
            return;
        }

        await SendAsync(new Response {
            Token = JwtCreator.CreateJwt(user)
        });
    }

    public class RequestValidator : Validator<Request> {
        public RequestValidator(PhoneNumberUtil phoneNumberUtil) {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(phoneNumberUtil.ValidatePhoneNumber)
                .WithMessage("phone number is invalid");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
