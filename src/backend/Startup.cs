using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using sproj.Data;
using sproj.Data.Entities;
using sproj.Endpoints;
using sproj.Services;

namespace sproj;

public static class Startup {
    public static void RegisterServices(this WebApplicationBuilder builder) {
        builder.Services.AddSerilog(logging => logging.ReadFrom.Configuration(builder.Configuration));

        if (builder.Environment.IsDevelopment()) {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        builder.Services.AddProblemDetails();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));

        builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<JwtOptions>((jwtBearerOptions, jwtOptions) => {
                jwtBearerOptions.MapInboundClaims = false;
                jwtBearerOptions.TokenValidationParameters = new() {
                    IssuerSigningKey = jwtOptions.SecurityKey,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        builder.Services.AddAuthorization(o => {
            o.AddPolicy("PhoneNotVerified",
                p => p.RequireAssertion(
                    ctx => ctx.User.HasClaim(c => c.Type == "isPhoneVerified" && c.Value == "False")));

            o.AddPolicy("PhoneVerified",
                p => p.RequireAssertion(
                    ctx => ctx.User.HasClaim(c => c.Type == "isPhoneVerified" && c.Value == "True")));
        });

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName)
            .Validate(o => o.Key != null, "Missing JWT Key").ValidateOnStart();
        builder.Services.AddSingleton(p => p.GetRequiredService<IOptions<JwtOptions>>().Value);

        builder.Services.AddScoped<PasswordHasher<User>>();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.AddFluentValidationEndpointFilter();

        builder.AddCustomServices();
    }

    private static void AddCustomServices(this WebApplicationBuilder builder) {
        builder.Services.AddSingleton<CodeVerifier>();
        builder.Services.AddScoped<JwtCreator>();

        if (builder.Environment.IsDevelopment()) {
            builder.Services.AddScoped<ISmsSender, DummySmsSender>();
        } else {
            builder.Services.AddScoped<ISmsSender, SmsSender>();
        }
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

        var api = app.MapGroup("/api").AddFluentValidationFilter();
        api.RegisterUserEndpoints();

        app.MapGet("/", () => "You're authorized").RequireAuthorization("PhoneVerified");
    }

    public static void ApplyMigrations(this WebApplication webApplication) {
        using (var scope = webApplication.Services.CreateScope()) {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
