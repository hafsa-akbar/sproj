using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using sproj.Models;

// ReSharper disable NotAccessedPositionalProperty.Global

namespace sproj.Services;

public class JwtCreatorService {
    private readonly JwtOptions _jwtOptions;

    public JwtCreatorService(JwtOptions jwtOptions) {
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
            expires: DateTime.Now.AddSeconds(_jwtOptions.Duration),
            signingCredentials: new SigningCredentials(_jwtOptions.SecurityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtResponse(new JwtSecurityTokenHandler().WriteToken(token), _jwtOptions.Duration);
    }
}

public record JwtResponse(string Token, int ExpiresIn);
