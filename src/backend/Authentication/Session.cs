using System.Collections.Concurrent;
using System.Security.Claims;
using sproj.Data;

namespace sproj.Authentication;

public class Session {
    public Session(ClaimsIdentity claims) {
        Claims = claims;
    }

    public Session(User user) {
        Claims = new ClaimsIdentity(new[] {
            new Claim("user_id", user.UserId.ToString()),
            new Claim("phone_number", user.PhoneNumber),
            new Claim("role", user.Role.ToString())
        }, "cookie", "user_id", "role");
    }

    public ClaimsIdentity Claims { get; }
}

public interface ISessionStore {
    Guid AddSession(Session session);
    void DeleteSession(Guid sessionId);
    Session? GetSession(Guid sessionId);
    void UpdateSession(Guid sessionId, Session session);
}

public class MemorySessionStore : ISessionStore {
    private readonly TimeSpan _sessionLifetime = TimeSpan.FromMinutes(15);
    private readonly ConcurrentDictionary<Guid, (DateTime ExpirationTime, Session Session)> _sessions = new();

    public Guid AddSession(Session session) {
        var sessionId = Guid.NewGuid();
        var expirationTime = DateTime.UtcNow.Add(_sessionLifetime);

        if (!_sessions.TryAdd(sessionId, (expirationTime, session)))
            throw new InvalidOperationException("Failed to add session.");

        return sessionId;
    }

    public void DeleteSession(Guid sessionId) {
        _sessions.TryRemove(sessionId, out _);
    }

    public Session? GetSession(Guid sessionId) {
        if (!_sessions.TryGetValue(sessionId, out var session))
            return null;

        if (session.ExpirationTime <= DateTime.UtcNow) {
            DeleteSession(sessionId);
            return null;
        }

        return new Session(session.Session.Claims.Clone());
    }

    public void UpdateSession(Guid sessionId, Session session) {
        var expirationTime = DateTime.UtcNow.Add(_sessionLifetime);

        if (_sessions.ContainsKey(sessionId))
            _sessions[sessionId] = (expirationTime, session);
        else
            AddSession(session);
    }

    public void CleanupExpiredSessions() {
        var expiredSessionIds = _sessions
            .Where(kvp => kvp.Value.ExpirationTime <= DateTime.UtcNow)
            .Select(kvp => kvp.Key)
            .ToList();

        expiredSessionIds.ForEach(DeleteSession);
    }
}