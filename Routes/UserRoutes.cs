using System.Data;
using Dapper;

namespace sproj.Routes;

public static class UserRoutes {
    public static RouteGroupBuilder UseUserRoutes(this RouteGroupBuilder routeGroup) {
        routeGroup.MapGet("/", async (IDbConnection connection) =>
            await connection.QueryAsync("select id, username from users;")
        );

        routeGroup.MapPost("/", async (User user, IDbConnection connection) => {
            await connection.ExecuteAsync("insert into users (username, password) values (@username, @password);",
                user);
            return Results.Created();
        });
        return routeGroup;
    }
}