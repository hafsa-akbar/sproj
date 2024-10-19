using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using sproj;
using sproj.Identity;
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
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IdentityDbContext>(provider => provider.GetRequiredService<AppDbContext>());

    builder.Services.AddIdentityCore<IdentityUser>().AddUserStore<UserStore>();

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    WebApplication app = builder.Build();

    if (!app.Environment.IsDevelopment()) app.UseExceptionHandler();
    app.UseStatusCodePages();

    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.Run();
} catch (HostAbortedException _) { } catch (Exception ex) {
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}