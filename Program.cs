using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(lc => lc.ReadFrom.Configuration(builder.Configuration));
    builder.Services.AddProblemDetails();

    WebApplication app = builder.Build();

    if (!app.Environment.IsDevelopment())
        app.UseExceptionHandler();
    
    app.UseStatusCodePages();

    app.UseSerilogRequestLogging();

    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}