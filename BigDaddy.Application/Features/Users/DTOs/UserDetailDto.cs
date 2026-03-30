namespace BigDaddy.Application.Features.Users.DTOs;

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

public class RoleAssignmentDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
}

public class TeamAssignmentDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
