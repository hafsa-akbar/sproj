using System.Data;
using System.Data.SQLite;
using Dapper;
using Serilog;
using sproj;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddSingleton<IDbConnection>(new SQLiteConnection("Data Source=database.db"));

WebApplication app = builder.Build();

InitDb(app.Services.GetRequiredService<IDbConnection>());

app.MapGet("/users", async (IDbConnection connection) => await connection.QueryAsync("select id, username from users;"));
app.MapPost("/users", async (User user, IDbConnection connection) => {
    await connection.ExecuteAsync("insert into users (username, password) values (@username, @password);", user);
    return Results.Created();
});

app.Run();

void InitDb(IDbConnection connection) {
    var sqlFilePath = "init_db.sql";
    var createTableQuery = File.ReadAllText(sqlFilePath);
    connection.Execute(createTableQuery);
}