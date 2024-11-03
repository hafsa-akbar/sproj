using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using sproj.Data.Entities;

namespace sproj.Services;

public class JwtCreator {
    private readonly JwtOptions _jwtOptions;

    public JwtCreator(JwtOptions jwtOptions) {
        _jwtOptions = jwtOptions;
    }

    public string CreateJwt(User user) {
        var jwtToken = JwtBearer.CreateToken(o => {
            o.SigningKey = _jwtOptions.Key!;
            o.ExpireAt = DateTime.Now.AddSeconds(_jwtOptions.Duration);

            o.User["username"] = user.Username;
        });

        return jwtToken;
    }
}
