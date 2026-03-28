namespace BigDaddy.Application.DTOs.Users;

public class UpdateUserDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public List<int> RoleIds { get; set; } = [];
    public List<int> TeamIds { get; set; } = [];
}
