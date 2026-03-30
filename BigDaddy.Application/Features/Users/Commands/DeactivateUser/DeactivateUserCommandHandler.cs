using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand>
{
    private readonly IUnitOfWork _uow;

    public DeactivateUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(DeactivateUserCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.Id, ct)
            ?? throw new NotFoundException("User", command.Id);

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }
}