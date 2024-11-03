using Microsoft.Extensions.Caching.Memory;

namespace sproj.Services;

// TODO: Use out of process database
public class CodeVerifier {
    private readonly MemoryCache _cache;
    private readonly Random _random;

    public CodeVerifier() {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _random = new Random();
    }

    public string CreateCode(string phoneNumber) {
        var code = GenerateRandomCode();

        _cache.Set(CacheKey(phoneNumber), code, new MemoryCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = new TimeSpan(0, 15, 0)
        });

        return code;
    }

    public bool VerifyCode(string phoneNumber, string code) {
        if (_cache.TryGetValue<string>(CacheKey(phoneNumber), out var storedCode))
            return code == storedCode;

        return false;
    }

    private string GenerateRandomCode(int length = 6) {
        var code = new char[length];
        for (var i = 0; i < length; i++) code[i] = (char)('0' + _random.Next(0, 10));

        return new string(code);
    }

    private string CacheKey(string phoneNumber) {
        return $"SMSCode_{phoneNumber}";
    }
}