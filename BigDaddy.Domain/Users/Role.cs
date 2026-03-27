using BigDaddy.Domain.Common;

namespace BigDaddy.Domain.Users;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;           // e.g. SystemAdministrator, Manager, EndUser
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<TeamRole> TeamRoles { get; set; } = [];
}