using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.DeactivateUser;

public class DeactivateUserCommand : ICommand
{
    public int Id { get; set; }
}
