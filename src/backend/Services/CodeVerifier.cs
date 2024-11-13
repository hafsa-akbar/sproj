using Microsoft.EntityFrameworkCore;
using sproj.Data;

namespace sproj.Services;

public class CodeVerifier {
    private readonly AppDbContext _appDbContext;
    private readonly Random _random;

    public CodeVerifier(AppDbContext appDbContext) {
        _appDbContext = appDbContext;
        _random = new Random();
    }

    public async Task<string> CreateCode(string phoneNumber) {
        var smsVerification = new SmsVerification {
            VerificationCode = GenerateRandomCode(),
            ExpiresAt = DateTime.Now.AddMinutes(5).ToUniversalTime()
        };

        var user = await _appDbContext.Users
            .Where(u => u.PhoneNumber == phoneNumber)
            .Include(u => u.SmsVerifications)
            .FirstAsync();

        user.SmsVerifications = smsVerification;

        await _appDbContext.SaveChangesAsync();

        return GenerateRandomCode();
    }

    public async Task<bool> VerifyCode(string phoneNumber, string code) {
        var user = await _appDbContext.Users
            .Where(u => u.PhoneNumber == phoneNumber)
            .Include(u => u.SmsVerifications)
            .FirstOrDefaultAsync();

        if (user?.SmsVerifications == null) return false;

        return user.SmsVerifications.VerificationCode == code &&
               user.SmsVerifications.ExpiresAt > DateTime.Now;
    }

    private string GenerateRandomCode(int length = 6) {
        var code = new char[length];
        for (var i = 0; i < length; i++) code[i] = (char)('0' + _random.Next(0, 10));

        return new string(code);
    }
}
