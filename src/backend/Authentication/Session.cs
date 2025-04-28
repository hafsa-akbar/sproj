using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using sproj.Data;

namespace sproj.Authentication;

public class Session {
    public required ClaimsIdentity ClaimsIdentity { get; init; }
}

public abstract class SessionStore {
    public abstract string CreateSession(Session session);
    public abstract void DeleteSession(string sessionId);
    public abstract Session? GetSession(string sessionId);
    public abstract void SetSession(string sessionId, Session session);

    protected TimeSpan SessionLifetime { get; } = TimeSpan.FromMinutes(40);

    public string CreateSession(User user) {
        var session = new Session {
            ClaimsIdentity = new ClaimsIdentity(new[] {
                new Claim("user_id", user.UserId.ToString()),
                new Claim("phone_number", user.PhoneNumber),
                new Claim("role", user.Role.ToString())
            }, "cookie", "user_id", "role")
        };

        return CreateSession(session);
    }

    protected virtual string GetSessionId() {
        Span<byte> byteArray = stackalloc byte[16];
        RandomNumberGenerator.Fill(byteArray);
        return BitConverter.ToString(byteArray.ToArray()).Replace("-", "");
    }
}

public class MemorySessionStore : SessionStore {
    private readonly ConcurrentDictionary<string, (DateTime ExpirationTime, Session Session)> _sessions = new();

    public override string CreateSession(Session session) {
        var sessionId = GetSessionId();
        var expirationTime = DateTime.UtcNow.Add(SessionLifetime);

        if (!_sessions.TryAdd(sessionId, (expirationTime, session)))
            throw new InvalidOperationException("Failed to add session.");

        return sessionId;
    }

    public override void DeleteSession(string sessionId) {
        _sessions.TryRemove(sessionId, out _);
    }

    public override Session? GetSession(string sessionId) {
        if (!_sessions.TryGetValue(sessionId, out var session))
            return null;

        if (session.ExpirationTime <= DateTime.UtcNow) {
            DeleteSession(sessionId);
            return null;
        }

        return new Session { ClaimsIdentity = session.Session.ClaimsIdentity.Clone() };
    }

    public override void SetSession(string sessionId, Session session) {
        var expirationTime = DateTime.UtcNow.Add(SessionLifetime);
        _sessions[sessionId] = (expirationTime, session);
    }
}