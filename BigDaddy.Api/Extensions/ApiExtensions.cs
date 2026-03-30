using BigDaddy.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace BigDaddy.Api.Extensions;

public static class ApiExtensions
{
    /// <summary>
    /// Registers permission-based authorization policies.
    /// Add new permission codes here.
    /// Convention: "module.screen.action" e.g. "invoices.list.view"
    /// </summary>
    public static IServiceCollection AddPermissionPolicies(
        this IServiceCollection services,
        params string[] permissionCodes)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

        services.AddAuthorization(options =>
        {
            // Role-based policies
            options.AddPolicy("SystemAdministrator", p => p.RequireRole("SystemAdministrator"));
            options.AddPolicy("Manager", p => p.RequireRole("SystemAdministrator", "Manager"));
            options.AddPolicy("EndUser", p => p.RequireRole("SystemAdministrator", "Manager", "EndUser"));

            // Permission-based policies — one policy per permission code
            foreach (var code in permissionCodes)
                options.AddPolicy(code, p => p.Requirements.Add(new PermissionRequirement(code)));
        });

        return services;
    }
}
