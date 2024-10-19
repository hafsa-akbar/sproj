using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityRole = sproj.Identity.IdentityRole;
using IdentityUser = sproj.Identity.IdentityUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using sproj.Identity;
using sproj;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(lc => lc.ReadFrom.Configuration(builder.Configuration));
    builder.Services.AddProblemDetails();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseLowerCaseNamingConvention());
    builder.Services.AddScoped<IdentityDbContext>(provider => provider.GetRequiredService<AppDbContext>());

    builder.Services.AddIdentityCore<IdentityUser>();
    builder.Services.AddTransient<IUserStore<IdentityUser>, UserStore>();
    builder.Services.AddTransient<IRoleStore<IdentityRole>, RoleStore>();

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };
        });

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }
    else app.UseExceptionHandler();

    app.UseStatusCodePages();
    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    app.MapPost("/register", async (RegisterModel input, UserManager<IdentityUser> userManager) => {
        var user = new IdentityUser { UserName = input.UserName };
        IdentityResult result = await userManager.CreateAsync(user, input.Password);

        if (result.Succeeded) {
            return Results.Ok();
        }

        return Results.BadRequest(result.Errors);
    });

    app.MapPost("/login",
        async Task<IResult> (LoginModel input, UserManager<IdentityUser> userManager, IConfiguration config) => {
            IdentityUser? user = await userManager.FindByNameAsync(input.UserName);
            if (user == null) return TypedResults.Unauthorized();

            var passwordCheck = await userManager.CheckPasswordAsync(user, input.Password);
            if (!passwordCheck) return TypedResults.Unauthorized();

            var claims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim("role", role)));

            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
            var token = new JwtSecurityToken(claims: claims,
                expires: DateTime.Now.AddSeconds(int.Parse(config["JWT:Duration"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return TypedResults.Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        });

    app.MapGet("/", () => "You are logged in!").RequireAuthorization();

    app.Run();
} catch (Exception ex) {
    if (ex is not HostAbortedException)
        Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}

internal record struct RegisterModel(string UserName, string Password);

internal record struct LoginModel(string UserName, string Password);