namespace BigDaddy.Domain.Users;

public class Permission
{
    public int Id { get; set; }
    public string Module { get; set; } = null!;         // e.g. Users, Reports, Dashboard
    public string Screen { get; set; } = null!;         // e.g. UserList, UserForm
    public string Action { get; set; } = null!;         // e.g. View, Create, Edit, Delete
    public string Code { get; set; } = null!;           // e.g. users.userlist.view
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}