namespace BigDaddy.Application.DTOs.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserProfileDto User { get; set; } = null!;
}
