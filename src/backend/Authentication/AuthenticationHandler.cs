using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace sproj.Authentication;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
    private readonly ISessionStore _sessionStore;

    public AuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISessionStore sessionStore)
        : base(options, logger, encoder) {
        _sessionStore = sessionStore;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        if (!Request.Cookies.TryGetValue("session", out var sessionCookie))
            return Task.FromResult(AuthenticateResult.Fail("Session cookie missing."));

        if (!Guid.TryParse(sessionCookie, out var sessionId))
            return Task.FromResult(AuthenticateResult.Fail("Invalid session cookie."));

        var session = _sessionStore.GetSession(sessionId);

        if (session == null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired session."));

        session.Claims.AddClaim(new Claim("session_id", sessionId.ToString()));
        var principal = new ClaimsPrincipal(session.Claims);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}