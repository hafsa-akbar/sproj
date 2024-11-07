using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;
using sproj.Data;
using sproj.Services;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace sproj.Features.Registration;

public class VerifySmsCodeEndpoint : Endpoint<VerifySmsCodeEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required CodeVerifier CodeVerifier { get; set; }

    public override void Configure() {
        Post("/users/verify-sms-code");
        Policy(p => p.RequireClaim("role", ((int)Data.Roles.Unregistered).ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var phoneNumber = User.FindFirst("phone_number")!.Value;

        var result = await CodeVerifier.VerifyCode(phoneNumber, req.Code);
        if (!result) {
            // TODO: inconsistent with problem details
            var error = new ErrorResponse([new ValidationFailure("code", "provided code is invalid")], 401);
            await error.ExecuteAsync(HttpContext);
            return;
        }

        var user = DbContext.Users.First(u => u.PhoneNumber == phoneNumber);
        user.RoleId = Data.Roles.Employer;
        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(new {
            Status = "success",
            Data = new {
                Token = JwtCreator.CreateJwt(user)
            }
        }));
    }

    public record struct Request(string Code);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.Code).NotEmpty().Length(6);
        }
    }
}
