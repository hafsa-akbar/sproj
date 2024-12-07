using FastEndpoints;
using FluentValidation;
using sproj.Authentication;
using sproj.Data;
using sproj.Services;

namespace sproj.Features.Registration;

public class Login : Endpoint<Login.Request, EmptyResponse> {
    public required AppDbContext DbContext { get; set; }
    public required ISessionStore SessionStore { get; set; }
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

        var sessionId = SessionStore.AddSession(new Session(user));
        HttpContext.Response.Cookies.Append("session", sessionId.ToString(), new CookieOptions {
            HttpOnly = true,
            Secure = true
        });

        await SendResultAsync(Results.Ok());
    }


    public record struct Request(string PhoneNumber, string Password);

    public class RequestValidator : Validator<Request> {
        public RequestValidator() {
            RuleFor(x => x.PhoneNumber).NotEmpty().Must(Utils.ValidatePhoneNumber);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }
}
