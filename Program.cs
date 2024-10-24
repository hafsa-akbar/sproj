using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using sproj.Endpoints;
using sproj.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.RegisterServices();

    WebApplication app = builder.Build();
    
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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddProblemDetails();
        
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<AppDbContext>();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString).UseLowerCaseNamingConvention());
        
        
        var jwtKey = builder.Configuration["JWT:Key"] ?? "00000000000000000000000000000000";
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
        builder.Services.AddAuthorization();
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

        RouteGroupBuilder api = app.MapGroup("api");
        api.RegisterUserEndpoints();
    }
}