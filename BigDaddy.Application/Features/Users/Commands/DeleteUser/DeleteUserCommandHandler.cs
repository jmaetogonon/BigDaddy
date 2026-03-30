using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(DeleteUserCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.Id, ct)
            ?? throw new NotFoundException("User", command.Id);

        _uow.Users.Remove(user);
        await _uow.SaveAsync(ct);
    }
}