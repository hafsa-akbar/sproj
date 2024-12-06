using FastEndpoints;
using FluentValidation;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Verification;

public class VerifySmsCodeEndpoint : Endpoint<VerifySmsCodeEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required JwtCreator JwtCreator { get; set; }
    public required CodeVerifier CodeVerifier { get; set; }

    public override void Configure() {
        Post("/verify/end-sms");
        Policy(p => p.RequireClaim("role", Role.Unregistered.ToString()));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct) {
        var phoneNumber = User.FindFirst("phone_number")!.Value;

        var result = await CodeVerifier.VerifyCode(phoneNumber, req.Code);
        if (!result) ThrowError(r => r.Code, "provided code is invalid", 401);

        var user = DbContext.Users.First(u => u.PhoneNumber == phoneNumber);
        user.Role = Role.Employer;

        user.UserPreferences = new UserPreferences {
            JobCategories = new List<JobCategory>(),
            JobTypes = new List<JobType>(),
            JobExperiences = new List<JobExperience>()
        };

        await DbContext.SaveChangesAsync();

        await SendResultAsync(Results.Ok(new {
            Token = JwtCreator.CreateJwt(user)
        }));
    }

    public record struct Request(string Code);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.Code).NotEmpty().Length(6);
        }
    }
}
