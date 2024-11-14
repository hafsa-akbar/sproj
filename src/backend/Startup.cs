using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using sproj.Data;
using sproj.Services;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace sproj;

public static class Startup {
    public static void RegisterServices(this WebApplicationBuilder builder) {
        var config = builder.Configuration;
        builder.Services.AddSerilog(logging => logging.ReadFrom.Configuration(config));

        builder.AddOptions();

        builder.Services.AddProblemDetails();
        builder.Services.AddFastEndpoints();

        builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = config["jwt:key"]);
        builder.Services.AddAuthorization();

        // TODO: How to move enum config to AppDbContext?
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString, optionsBuilder => {
            optionsBuilder.MapEnum<Role>("role");
            optionsBuilder.MapEnum<JobCategory>("job_category");
            optionsBuilder.MapEnum<JobExperience>("job_experience");
            optionsBuilder.MapEnum<JobType>("job_type");
            optionsBuilder.MapEnum<UserGender>("user_gender");
            optionsBuilder.MapEnum<JobGender>("job_gender");
        }));

        builder.AddCustomServices();
    }

    private static void AddOptions(this WebApplicationBuilder builder) {
        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName)
            .Validate(o => o.Key != null, "no jwt secret key").ValidateOnStart();
        builder.Services.AddSingleton(p => p.GetRequiredService<IOptions<JwtOptions>>().Value);

        if (!builder.Environment.IsDevelopment()) {
            builder.Services.AddOptions<TwilioOptions>().BindConfiguration(TwilioOptions.SectionName)
                .Validate(o => o.AccountSid != null, "no account sid")
                .Validate(o => o.AuthToken != null, "no auth token")
                .Validate(o => o.PhoneNumber != null, "no twilio phone number")
                .ValidateOnStart();
            builder.Services.AddSingleton(p => p.GetRequiredService<IOptions<TwilioOptions>>().Value);
        }
    }

    private static void AddCustomServices(this WebApplicationBuilder builder) {
        builder.Services.AddScoped<CodeVerifier>();
        builder.Services.AddSingleton<JwtCreator>();

        if (!builder.Environment.IsDevelopment())
            builder.Services.AddScoped<ICnicVerificationService, CnicVerificationService>();
        else builder.Services.AddSingleton<ICnicVerificationService, DummyCnicVerificationService>();

        if (builder.Environment.IsDevelopment()) builder.Services.AddSingleton<ISmsSender, DummySmsSender>();
        else builder.Services.AddSingleton<ISmsSender, SmsSender>();
    }

    public static void RegisterMiddleware(this WebApplication app) {
        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseSerilogRequestLogging();
        app.UseStatusCodePages();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseFastEndpoints(x => x.Errors.UseProblemDetails());
    }

    public static void ApplyMigrations(this WebApplication webApplication) {
        using (var scope = webApplication.Services.CreateScope()) {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
