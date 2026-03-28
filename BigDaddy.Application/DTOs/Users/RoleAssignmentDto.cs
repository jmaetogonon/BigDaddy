namespace BigDaddy.Application.DTOs.Users;

public class RoleAssignmentDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
}
