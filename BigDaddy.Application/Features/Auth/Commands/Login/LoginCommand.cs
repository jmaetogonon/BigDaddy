using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Features.Auth.DTOs;
using System.ComponentModel.DataAnnotations;

namespace BigDaddy.Application.Features.Auth.Commands.Login;

public class LoginCommand : ICommand<LoginResponseDto>
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
