using System.Data;
using System.Data.SQLite;
using Dapper;
using Serilog;
using sproj.Routes;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(lc => lc.ReadFrom.Configuration(builder.Configuration));
    builder.Services.AddProblemDetails();

    if (builder.Environment.IsDevelopment()) {
        var connectionString = builder.Configuration.GetConnectionString("SQLite");
        builder.Services.AddSingleton<IDbConnection>(new SQLiteConnection(connectionString));
    } else {
        // TODO: Move to production database
        var connectionString = builder.Configuration.GetConnectionString("SQLite");
        builder.Services.AddSingleton<IDbConnection>(new SQLiteConnection(connectionString));
    }

    WebApplication app = builder.Build();

    if (!app.Environment.IsDevelopment()) app.UseExceptionHandler();
    app.UseStatusCodePages();

    InitDb(app.Services.GetRequiredService<IDbConnection>());

    app.UseSerilogRequestLogging();

    app.MapGroup("/users").UseUserRoutes();

    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
    Log.CloseAndFlush();
}

void InitDb(IDbConnection connection) {
    var sqlFilePath = "init_db.sql";
    var createTableQuery = File.ReadAllText(sqlFilePath);
    connection.Execute(createTableQuery);

    Log.Information("Database successfully initialized");
}