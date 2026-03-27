namespace BigDaddy.Application.Models.Auth;

public class UserProfile 
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<string> Teams { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}