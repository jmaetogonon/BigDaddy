
using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : ICommand
{
    public string Jti { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime TokenExpiry { get; set; }
}
