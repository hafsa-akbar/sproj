using Microsoft.Extensions.Caching.Memory;

namespace sproj.Services;

public class CodeVerifier {
    private readonly MemoryCache _cache;
    private readonly Random _random;

    public CodeVerifier() {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _random = new Random();
    }

    public int CreateCode(string username) {
        var code = GenerateRandomCode();

        _cache.Set(CacheKey(username), code, new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 15, 0)
        });

        return code;
    }

    public bool VerifyCode(string username, int code) {
        if (_cache.TryGetValue<int>(CacheKey(username), out var storedCode))
            return code == storedCode;

        return false;
    }

    private int GenerateRandomCode(int length = 6) {
        var code = new char[length];
        for (var i = 0; i < length; i++) code[i] = (char)('0' + _random.Next(0, 10));

        return int.Parse(code);
    }

    private string CacheKey(string username) {
        return $"SMSCode_{username}";
    }
}