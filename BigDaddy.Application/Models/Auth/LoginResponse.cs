namespace BigDaddy.Application.Models.Auth;

public class LoginResponse 
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserProfile  User { get; set; } = null!;
}
