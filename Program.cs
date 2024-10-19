using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using sproj.Identity;
using sproj.Models;
using IdentityRole = sproj.Identity.IdentityRole;
using IdentityUser = sproj.Identity.IdentityUser;

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

    builder.Services.AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };
        });
    builder.Services.AddAuthorization();

    builder.Services.AddControllers();

    WebApplication app = builder.Build();

    app.UseRouting();

    if (app.Environment.IsDevelopment()) {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }
    else app.UseExceptionHandler();

    app.UseStatusCodePages();
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapGet("/", () => "You are logged in!").RequireAuthorization();

    app.Run();
} catch (Exception ex) {
    if (ex is not HostAbortedException)
        Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}