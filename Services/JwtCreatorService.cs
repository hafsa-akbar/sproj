using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using sproj.Models;

// ReSharper disable NotAccessedPositionalProperty.Global

namespace sproj.Services;

public class JwtCreatorService {
    private readonly JwtOptions jwtOptions;

    public JwtCreatorService(JwtOptions jwtOptions) {
        this.jwtOptions = jwtOptions;
    }

    public JwtResponse CreateJwt(User user) {
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

        return new JwtResponse(new JwtSecurityTokenHandler().WriteToken(token), jwtOptions.Duration);
    }
}

public record JwtResponse(string token, int expires_in);
