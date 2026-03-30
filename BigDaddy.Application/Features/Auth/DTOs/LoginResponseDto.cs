namespace BigDaddy.Application.Features.Auth.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserProfileDto User { get; set; } = null!;
}
