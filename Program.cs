using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using sproj.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddServices(builder.Configuration);

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
    app.MapGet("/", (ClaimsPrincipal user) => "You are logged in!").RequireAuthorization();

    app.Run();
} catch (HostAbortedException _) { } catch (Exception ex) {
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}

public static class ServiceCollectionExtensions {
    private static IConfiguration _config;

    public static void AddServices(this IServiceCollection services, IConfiguration config) {
        _config = config;

        services
            .AddSerilog()
            .AddSwagger()
            .AddProblemDetails()
            .AddControllers();

        services
            .AddDataServices()
            .AddAuthServices();
    }

    private static IServiceCollection AddSerilog(this IServiceCollection services) {
        services.AddSerilog(logging => logging.ReadFrom.Configuration(_config));
        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services) {
        services.AddEndpointsApiExplorer()
            .AddSwaggerGen();
        return services;
    }

    private static IServiceCollection AddDataServices(this IServiceCollection services) {
        var connectionString = _config.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
                .UseLowerCaseNamingConvention());

        services.AddIdentityCore<IdentityUser>()
            .AddEntityFrameworkStores<AppDbContext>();

        return services;
    }

    private static IServiceCollection AddAuthServices(this IServiceCollection services) {
        var jwtKey = _config["JWT:Key"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
                options.MapInboundClaims = false;
            });

        services.AddAuthorization();
        return services;
    }
}