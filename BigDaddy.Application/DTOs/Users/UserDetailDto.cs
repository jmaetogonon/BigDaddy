namespace BigDaddy.Application.DTOs.Users;

public class UserDetailDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<RoleAssignmentDto> Roles { get; set; } = [];
    public List<TeamAssignmentDto> Teams { get; set; } = [];
}
