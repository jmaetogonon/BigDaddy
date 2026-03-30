using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommand : ICommand
{
    public int UserId { get; set; }   // set by controller from route
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
