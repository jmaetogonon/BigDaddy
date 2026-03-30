using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Users.Commands.UnlockUser;

public class UnlockUserCommandHandler : ICommandHandler<UnlockUserCommand>
{
    private readonly IUnitOfWork _uow;

    public UnlockUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(UnlockUserCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.Id, ct)
            ?? throw new NotFoundException("User", command.Id);

        user.IsLocked = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }
}