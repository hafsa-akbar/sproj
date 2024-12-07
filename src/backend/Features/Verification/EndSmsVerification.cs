using System.Security.Claims;
using FastEndpoints;
using FluentValidation;
using sproj.Authentication;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Verification;

public class VerifySmsCodeEndpoint : Endpoint<VerifySmsCodeEndpoint.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required ISessionStore SessionStore { get; set; }
    public required CodeVerifier CodeVerifier { get; set; }

    public override void Configure() {
        Post("/verify/end-sms");
        Policies("Unregistered");
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

        var sessionId = Guid.Parse(User.FindFirst("session_id")!.Value);
        var session = SessionStore.GetSession(sessionId)!;

        session.Claims.RemoveClaim(session.Claims.FindFirst("role"));
        session.Claims.AddClaim(new Claim("role", user.Role.ToString()));

        SessionStore.UpdateSession(sessionId, session);

        await SendResultAsync(Results.Ok());
    }

    public record struct Request(string Code);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.Code).NotEmpty().Length(6);
        }
    }
}