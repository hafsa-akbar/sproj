using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using sproj.Models;
using sproj.Services;

namespace sproj.Endpoints;

// TODO: Add validation
public static class UserEndpoints {
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/user");

        group.MapPost("/register", RegisterEndpoint);
        group.MapPost("/login", LoginEndpoint);

        group.MapPost("/send-verification-sms", SendVerificationSMSEndpoint).RequireAuthorization("PhoneNotVerified");
        group.MapPost("/verify-sms", VerifySMSEndpoint).RequireAuthorization("PhoneNotVerified");
    }

    public static async Task<IResult> RegisterEndpoint(RegisterRequest input, AppDbContext dbContext,
        PasswordHasher<User> passwordHasher, JwtCreatorService jwtCreator) {
        if (await dbContext.Users.AnyAsync(u => u.Username == input.UserName))
            return Results.BadRequest("Username is already taken");

        var user = new User { Username = input.UserName, Password = string.Empty, PhoneNumber = input.PhoneNumber };
        user.Password = passwordHasher.HashPassword(user, input.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return Results.Ok(jwtCreator.CreateJwt(user));
    }

    public static IResult LoginEndpoint(LoginRequest input, AppDbContext dbContext, PasswordHasher<User> passwordHasher,
        JwtCreatorService jwtCreator) {
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
    public static IResult SendVerificationSMSEndpoint(ClaimsPrincipal claimsPrincipal, AppDbContext dbContext,
        CodeVerificationService codeVerificationService, ISMSService smsService) {
        var username = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

        var user = dbContext.Users.First(u => u.Username == username);
        var code = codeVerificationService.CreateCode(username);
        smsService.SendCode(user.PhoneNumber, code);

        return Results.Ok(new {
            message = "Verification code sent!"
        });
    }

    public static async Task<IResult> VerifySMSEndpoint(int code, AppDbContext dbContext, ClaimsPrincipal claimsPrincipal, CodeVerificationService codeVerificationService) {
        var username = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;

        var result = codeVerificationService.VerifyCode(username, code);

        if (result) {
            var user = dbContext.Users.First(u => u.Username == username);
            user.isPhoneVerified = true;
            await dbContext.SaveChangesAsync();
            return Results.Ok(new {
                message = "Verification successful!"
            });
        }

        return Results.Unauthorized();
    }

    public record struct RegisterRequest(string UserName, string Password, string PhoneNumber);
    public record struct LoginRequest(string UserName, string Password);
}
