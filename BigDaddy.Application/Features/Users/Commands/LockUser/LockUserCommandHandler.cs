using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Users.Commands.LockUser;

public class LockUserCommandHandler : ICommandHandler<LockUserCommand>
{
    private readonly IUnitOfWork _uow;

    public LockUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(LockUserCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.Id, ct)
            ?? throw new NotFoundException("User", command.Id);

        user.IsLocked = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }
}