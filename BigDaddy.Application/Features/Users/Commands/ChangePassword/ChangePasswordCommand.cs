using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommand : ICommand
{
    public int UserId { get; set; }   // set by controller from JWT
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
