using Microsoft.AspNetCore.Authorization;
using sproj.Data;

namespace sproj.Authentication;

public static class AuthorizationExtensions {
    public static void AddAuthorizationPolicies(this AuthorizationOptions options) {
        options.AddPolicy("Unregistered", policy => policy.RequireRole(Role.Unregistered.ToString()));
        options.AddPolicy("Employer", policy => policy.RequireRole(Role.Employer.ToString()));
        options.AddPolicy("Worker", policy => policy.RequireRole(Role.Worker.ToString()));

        options.AddPolicy("EmployerOrWorker", policy =>
            policy.RequireAssertion(context => !context.User.IsInRole(Role.Unregistered.ToString())));
    }
}