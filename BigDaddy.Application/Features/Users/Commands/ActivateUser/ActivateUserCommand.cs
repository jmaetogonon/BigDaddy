using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.ActivateUser;

public class ActivateUserCommand : ICommand
{
    public int Id { get; set; }
}
