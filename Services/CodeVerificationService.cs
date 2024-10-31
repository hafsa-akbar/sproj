using Microsoft.Extensions.Caching.Memory;

namespace sproj.Services;

public class CodeVerificationService {
    private readonly MemoryCache cache;
    private readonly Random random;

    public CodeVerificationService() {
        cache = new MemoryCache(new MemoryCacheOptions());
        random = new Random();
    }

    public int CreateCode(string username) {
        var code = GenerateRandomCode();

        cache.Set(CacheKey(username), code, new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 15, 0)
        });

        return code;
    }

    public bool VerifyCode(string username, int code) {
        if (cache.TryGetValue<int>(CacheKey(username), out var storedCode))
            return code == storedCode;

        return false;
    }

    private int GenerateRandomCode(int length = 6) {
        var code = new char[length];
        for (var i = 0; i < length; i++) code[i] = (char)('0' + random.Next(0, 10));

        return int.Parse(code);
    }

    private string CacheKey(string username) {
        return $"SMSCode_{username}";
    }
}
