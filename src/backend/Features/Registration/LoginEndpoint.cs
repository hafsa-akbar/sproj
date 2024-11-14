using FastEndpoints;
using FluentValidation;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class LoginEndpoint : Endpoint<LoginEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required PasswordHasher PasswordHasher { get; set; }

    public override void Configure() {
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var normalizedPhoneNumber = Utils.NormalizePhoneNumber(req.PhoneNumber);

        var user = DbContext.Users.SingleOrDefault(u => u.PhoneNumber == normalizedPhoneNumber);
        if (user is null) {
            await SendUnauthorizedAsync();
            return;
        }

        if (!PasswordHasher.VerifyHashedPassword(user.Password, req.Password)) {
            await SendUnauthorizedAsync();
            return;
        }

        await SendResultAsync(Results.Ok(new {
            Token = JwtCreator.CreateJwt(user)
        }));
    }


    public record struct Request(string PhoneNumber, string Password);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber)
                .WithMessage("phone number is invalid");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
