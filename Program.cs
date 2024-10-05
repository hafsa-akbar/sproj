using System.Data;
using System.Data.SQLite;
using Dapper;
using Serilog;
using Serilog.Events;
using sproj;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.Console()
    .CreateLogger();

try {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Register services
    builder.Host.UseSerilog();
    builder.Services.AddSingleton<IDbConnection>(new SQLiteConnection("Data Source=db.sqlite"));

    WebApplication app = builder.Build();

    InitDb(app.Services.GetRequiredService<IDbConnection>());

    // Middleware
    app.UseSerilogRequestLogging();

    app.MapGet("/users",
        async (IDbConnection connection) => await connection.QueryAsync("select id, username from users;"));
    app.MapPost("/users", async (User user, IDbConnection connection) => {
        await connection.ExecuteAsync("insert into users (username, password) values (@username, @password);", user);
        return Results.Created();
    });

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
}
