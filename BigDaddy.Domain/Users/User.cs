using BigDaddy.Domain.Common;

namespace BigDaddy.Domain.Users;

public class User : IAuditableEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsLocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserTeam> UserTeams { get; set; } = [];
}
