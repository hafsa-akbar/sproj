using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration config) {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel input) {
        var user = new IdentityUser { UserName = input.UserName };
        IdentityResult result = await _userManager.CreateAsync(user, input.Password);

        if (result.Succeeded)
            return Ok();

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel input) {
        IdentityUser? user = await _userManager.FindByNameAsync(input.UserName);
        if (user == null) return Unauthorized();

        var passwordCheck = await _userManager.CheckPasswordAsync(user, input.Password);
        if (!passwordCheck) return Unauthorized();

        var claims = new List<Claim> {
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddSeconds(int.Parse(_config["JWT:Duration"])),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { jwt = tokenString });
    }

    public record struct RegisterModel(string UserName, string Password);
    public record struct LoginModel(string UserName, string Password);
}