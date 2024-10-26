using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using sproj;
using sproj.Endpoints;
using sproj.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    var builder = WebApplication.CreateBuilder(args);

    builder.RegisterServices();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope()) {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.Migrate();
    }

    app.RegisterMiddleware();

    app.Run();
} catch (HostAbortedException) { } catch (Exception ex) {
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}

public static class StartupExtensions {
    public static void RegisterServices(this WebApplicationBuilder builder) {
        builder.Services.AddSerilog(logging => logging.ReadFrom.Configuration(builder.Configuration));
        builder.Services.AddProblemDetails();

        if (builder.Environment.IsDevelopment()) {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString).UseLowerCaseNamingConvention());

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        builder.Services.AddAuthorization();

        builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<JwtOptions>((jwtBearerOptions, jwtOptions) => {
                jwtBearerOptions.MapInboundClaims = false;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters {
                    IssuerSigningKey = jwtOptions.SecurityKey,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName)
            .Validate(o => o.Key != null, "Missing JWT Key").ValidateOnStart();
        builder.Services.AddSingleton(p => p.GetRequiredService<IOptions<JwtOptions>>().Value);
    }

    public static void RegisterMiddleware(this WebApplication app) {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseAuthentication();
        app.UseAuthorization();

        var api = app.MapGroup("api");
        api.RegisterUserEndpoints();

        app.MapGet("/", () => "You're authorized").RequireAuthorization();
    }
}