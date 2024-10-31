using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sproj.Models;

namespace sproj.Endpoints;

// TODO: Add validation
public static class UserEndpoints {
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/user");

        group.MapPost("/register", RegisterEndpoint);
        group.MapPost("/login", LoginEndpoint);

        group.MapPost("/send-verification-sms", VerifyPhoneEndpoint).RequireAuthorization(policy =>
            policy.RequireAssertion(ctx => ctx.User.HasClaim(c => c.Type == "isPhoneVerified" && c.Value == "false")));
        // group.MapPost("/verify-phone", LoginEndpoint);
    }

    public static async Task<IResult> RegisterEndpoint(RegisterRequest input,
        AppDbContext dbContext,
        PasswordHasher<User> passwordHasher) {
        if (await dbContext.Users.AnyAsync(u => u.Username == input.UserName))
            return Results.BadRequest("Username is already taken");

        var user = new User { Username = input.UserName, Password = string.Empty, PhoneNumber = input.PhoneNumber };
        user.Password = passwordHasher.HashPassword(user, input.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    // TODO: Timing attack vulnerability
    public static IResult LoginEndpoint(LoginRequest input, AppDbContext dbContext,
        PasswordHasher<User> passwordHasher, JwtOptions jwtOptions) {
        var user = dbContext.Users.SingleOrDefault(u => u.Username == input.UserName);
        if (user == null) return Results.Unauthorized();

        var passwordCheck = passwordHasher.VerifyHashedPassword(user, user.Password, input.Password);
        if (passwordCheck == PasswordVerificationResult.Failed) return Results.Unauthorized();

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("isPhoneVerified", user.isPhoneVerified.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddSeconds(jwtOptions.Duration),
            signingCredentials: new SigningCredentials(jwtOptions.SecurityKey, SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new {
            token = tokenString, expires_in = jwtOptions.Duration, token_type = JwtBearerDefaults.AuthenticationScheme
        });
    }

    public static IResult VerifyPhoneEndpoint() {
        return Results.Ok();
    }

    public record struct RegisterRequest(string UserName, string Password, string PhoneNumber);

    public record struct LoginRequest(string UserName, string Password);
}
