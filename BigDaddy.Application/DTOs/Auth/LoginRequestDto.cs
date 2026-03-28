using System.ComponentModel.DataAnnotations;

namespace BigDaddy.Application.DTOs.Auth;

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
