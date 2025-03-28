using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sproj.Data;

namespace sproj.Authentication;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly SessionStore _sessionStore;
    private readonly ILogger<AuthenticationHandler> _logger;

    public AuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        SessionStore sessionStore)
        : base(options, logger, encoder) {
        _sessionStore = sessionStore;
        _logger = logger.CreateLogger<AuthenticationHandler>();
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        _logger.LogInformation("Starting authentication process");
        
        if (!Request.Cookies.TryGetValue("session", out var sessionId)) {
            _logger.LogWarning("No session cookie found in request");
            return Task.FromResult(AuthenticateResult.Fail("Session cookie missing."));
        }
        
        _logger.LogInformation("Found session cookie with ID: {SessionId}", sessionId);

        var session = _sessionStore.GetSession(sessionId);

        if (session == null) {
            _logger.LogWarning("Invalid or expired session: {SessionId}", sessionId);
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired session."));
        }

        _logger.LogInformation("Valid session found for user: {UserId}", session.ClaimsIdentity.Name);
        session.ClaimsIdentity.AddClaim(new Claim("session_id", sessionId));
        
        var principal = new ClaimsPrincipal(session.ClaimsIdentity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class AuthorizationExtensions {
    public static void AddAuthorizationPolicies(this AuthorizationOptions options) {
        options.AddPolicy("Unregistered", policy => policy.RequireRole(Role.Unregistered.ToString()));
        options.AddPolicy("Employer", policy => policy.RequireRole(Role.Employer.ToString()));
        options.AddPolicy("Worker", policy => policy.RequireRole(Role.Worker.ToString()));

        options.AddPolicy("EmployerOrWorker", policy =>
            policy.RequireAssertion(context => !context.User.IsInRole(Role.Unregistered.ToString())));
    }
}
