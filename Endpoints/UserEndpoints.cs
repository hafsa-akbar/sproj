using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace sproj.Endpoints;

public static class UserEndpoints {
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app) {
        RouteGroupBuilder group = app.MapGroup("user");

        group.MapPost("/register", RegisterEndpoint);
        group.MapPost("/login", LoginEndpoint);
    }

    public static async Task<IResult> RegisterEndpoint(RegisterDTO input,
        UserManager<IdentityUser> userManager) {
        var user = new IdentityUser { UserName = input.UserName };
        IdentityResult result = await userManager.CreateAsync(user, input.Password);

        if (result.Succeeded)
            return Results.Ok();

        return Results.BadRequest(result.Errors);
    }

    public static async Task<IResult> LoginEndpoint(LoginDTO input, UserManager<IdentityUser> userManager,
        JWTOptions jwtOptions) {
        IdentityUser? user = await userManager.FindByNameAsync(input.UserName);
        if (user == null) return Results.Unauthorized();

        var passwordCheck = await userManager.CheckPasswordAsync(user, input.Password);
        if (!passwordCheck) return Results.Unauthorized();

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

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

    public record struct RegisterDTO(string UserName, string Password);

    public record struct LoginDTO(string UserName, string Password);
}