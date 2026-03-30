namespace BigDaddy.Application.Features.Users.DTOs;

public class UserListItemDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<string> Teams { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
