using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using sproj.Models;
using sproj.Services;

namespace sproj.Endpoints;

public static class UserEndpoints {
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/user");

        group.MapPost("/register", RegisterEndpoint);
        group.MapPost("/login", LoginEndpoint);

        group.MapPost("/send-verification-sms", SendVerificationSmsEndpoint).RequireAuthorization("PhoneNotVerified");
        group.MapPost("/verify-sms", VerifySmsEndpoint).RequireAuthorization("PhoneNotVerified");
    }

    public record struct RegisterRequest(string UserName, string Password, string PhoneNumber);

    public static async Task<IResult> RegisterEndpoint(RegisterRequest input, AppDbContext dbContext,
        PasswordHasher<User> passwordHasher, JwtCreator jwtCreator) {
        if (await dbContext.Users.AnyAsync(u => u.Username == input.UserName))
            return Results.BadRequest("Username is already taken");

        var user = new User { Username = input.UserName, Password = string.Empty, PhoneNumber = input.PhoneNumber };
        user.Password = passwordHasher.HashPassword(user, input.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return Results.Ok(jwtCreator.CreateJwt(user));
    }

    public record struct LoginRequest(string UserName, string Password);

    public static IResult LoginEndpoint(LoginRequest input, AppDbContext dbContext, PasswordHasher<User> passwordHasher,
        JwtCreator jwtCreator) {
        var dummyUser = new User {
            Password = "",
            Username = null!,
            PhoneNumber = null!
        };

        var user = dbContext.Users.SingleOrDefault(u => u.Username == input.UserName) ?? dummyUser;

        var passwordCheck = passwordHasher.VerifyHashedPassword(user, user.Password, input.Password);
        if (passwordCheck == PasswordVerificationResult.Failed) return Results.Unauthorized();

        return Results.Ok(jwtCreator.CreateJwt(user));
    }

    // TODO: Add rate limit
    public static IResult SendVerificationSmsEndpoint(ClaimsPrincipal claimsPrincipal, AppDbContext dbContext,
        CodeVerifier codeVerifier, ISmsSender smsSender) {
        var username = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

        var user = dbContext.Users.First(u => u.Username == username);
        var code = codeVerifier.CreateCode(username);
        smsSender.SendCode(user.PhoneNumber, code);

        return Results.Ok(new {
            message = "Verification code sent!"
        });
    }

    public static async Task<IResult> VerifySmsEndpoint(int code, AppDbContext dbContext,
        ClaimsPrincipal claimsPrincipal, CodeVerifier codeVerifier) {
        var username = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

        var result = codeVerifier.VerifyCode(username, code);

        if (result) {
            var user = dbContext.Users.First(u => u.Username == username);
            user.IsPhoneVerified = true;
            await dbContext.SaveChangesAsync();
            return Results.Ok(new {
                message = "Verification successful!"
            });
        }

        return Results.Unauthorized();
    }
}
