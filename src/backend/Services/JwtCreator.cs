using FastEndpoints.Security;
using sproj.Data;

namespace sproj.Services;

public class JwtCreator {
    private readonly JwtOptions _jwtOptions;

    public JwtCreator(JwtOptions jwtOptions) {
        _jwtOptions = jwtOptions;
    }

    public string CreateJwt(User user) {
        var jwtToken = JwtBearer.CreateToken(o => {
            o.SigningKey = _jwtOptions.Key;
            o.ExpireAt = DateTime.Now.AddSeconds(_jwtOptions.Duration);

            o.User["user_id"] = user.UserId.ToString();
            o.User["phone_number"] = user.PhoneNumber;
            o.User["role"] = user.Role.ToString();
        });

        return jwtToken;
    }
}
