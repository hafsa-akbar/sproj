using Microsoft.AspNetCore.Identity;
using sproj.Data;

namespace sproj.Services;

public class PasswordHasher {
    private readonly IPasswordHasher<User> _passwordHasher;
    public PasswordHasher(IPasswordHasher<User> passwordHasher) => _passwordHasher = passwordHasher;

    public string HashPassword(string password) {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword) {
        return _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword) ==
               PasswordVerificationResult.Success;
    }
}
