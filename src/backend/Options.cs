using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace sproj;

public class JwtOptions {
    public const string SectionName = "JWT";

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Key { get; set; }
    public int Duration { get; set; } = 3600;

    public SecurityKey SecurityKey =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key!));
}