using Microsoft.AspNetCore.Authorization;

namespace BigDaddy.Api.Authorization;

/// <summary>Usage: [HasPermission("users.list.view")]</summary>
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permissionCode) : base(permissionCode) { }
}