using Microsoft.AspNetCore.Authorization;

namespace BigDaddy.Api.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission = context.User.Claims
            .Any(c => c.Type == "permission" && c.Value == requirement.PermissionCode);

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
