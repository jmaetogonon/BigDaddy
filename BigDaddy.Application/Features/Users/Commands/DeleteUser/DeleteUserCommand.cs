using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : ICommand
{
    public int Id { get; set; }
}