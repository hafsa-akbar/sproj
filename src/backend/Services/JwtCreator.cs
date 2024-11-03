using FastEndpoints.Security;
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

            o.User["phoneNumber"] = user.PhoneNumber;
            o.User["isPhoneVerified"] = user.IsPhoneVerified.ToString().ToLower();
        });

        return jwtToken;
    }
}