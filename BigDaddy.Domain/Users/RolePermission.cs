namespace BigDaddy.Domain.Users;

public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}