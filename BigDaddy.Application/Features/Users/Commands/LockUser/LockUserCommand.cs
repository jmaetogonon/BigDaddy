using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.LockUser;

public class LockUserCommand : ICommand
{
    public int Id { get; set; }
}
