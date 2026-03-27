using Microsoft.AspNetCore.Authorization;

namespace BigDaddy.Api.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permissionCode)
        : base(permissionCode)
    {
    }
}
