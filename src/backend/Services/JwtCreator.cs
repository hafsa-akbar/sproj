using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using sproj.Models;

namespace sproj.Services;

public class JwtCreator {
    private readonly JwtOptions _jwtOptions;

    public JwtCreator(JwtOptions jwtOptions) {
        _jwtOptions = jwtOptions;
    }

    public JwtResponse CreateJwt(User user) {
        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("isPhoneVerified", user.IsPhoneVerified.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: new(_jwtOptions.SecurityKey, SecurityAlgorithms.HmacSha256),
            expires: DateTime.Now.AddSeconds(_jwtOptions.Duration)
        );

        return new JwtResponse(new JwtSecurityTokenHandler().WriteToken(token), _jwtOptions.Duration);
    }
}

// ReSharper disable NotAccessedPositionalProperty.Global
public record JwtResponse(string Token, int ExpiresIn);
