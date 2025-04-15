using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using sproj.Data;

namespace sproj.Authentication;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly SessionStore _sessionStore;

    public AuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        SessionStore sessionStore)
        : base(options, logger, encoder) {
        _sessionStore = sessionStore;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        if (!Request.Cookies.TryGetValue("session", out var sessionId)) {
            Console.WriteLine("[AuthHandler] Session cookie missing");
            return Task.FromResult(AuthenticateResult.Fail("Session cookie missing."));
        }
    
        Console.WriteLine($"[AuthHandler] Received session ID: {sessionId}");
    
        var session = _sessionStore.GetSession(sessionId);
        if (session == null) {
            Console.WriteLine($"[AuthHandler] No session found for ID: {sessionId}");
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired session."));
        }
    
        Console.WriteLine($"[AuthHandler] Session found. Role: {session.ClaimsIdentity.FindFirst("role")?.Value}");
    
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
