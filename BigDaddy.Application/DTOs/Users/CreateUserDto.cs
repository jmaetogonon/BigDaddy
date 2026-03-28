namespace BigDaddy.Application.DTOs.Users;

public class CreateUserDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public List<int> RoleIds { get; set; } = [];
    public List<int> TeamIds { get; set; } = [];
}
