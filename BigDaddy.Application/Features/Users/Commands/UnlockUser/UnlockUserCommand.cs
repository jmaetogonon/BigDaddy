using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.UnlockUser;

public class UnlockUserCommand : ICommand
{
    public int Id { get; set; }
}
