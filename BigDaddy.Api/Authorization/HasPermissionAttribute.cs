using Microsoft.AspNetCore.Authorization;

namespace BigDaddy.Api.Authorization;


/// <summary>
/// Requires the authenticated user to have the specified permission code in their JWT.
/// Usage: [HasPermission("users.list.view")]
/// </summary>
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permissionCode) : base(permissionCode) { }
}