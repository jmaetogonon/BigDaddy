using BigDaddy.Domain.Common;

namespace BigDaddy.Domain.Users;

public class User : BaseEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsLocked { get; set; } = false;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserTeam> UserTeams { get; set; } = [];
}
