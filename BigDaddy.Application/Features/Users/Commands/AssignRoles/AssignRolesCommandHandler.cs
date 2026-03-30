using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Exceptions;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Features.Users.Commands.AssignRoles;

public class AssignRolesCommandHandler : ICommandHandler<AssignRolesCommand>
{
    private readonly IUnitOfWork _uow;

    public AssignRolesCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(AssignRolesCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.UserId, ct)
            ?? throw new NotFoundException("User", command.UserId);

        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();

        foreach (var roleId in command.RoleIds.Distinct().Where(r => !existingRoleIds.Contains(r)))
        {
            if (!await _uow.Users.RoleExistsAsync(roleId, ct))
                throw new NotFoundException("Role", roleId);

            _uow.Users.AddUserRole(new UserRole { UserId = command.UserId, RoleId = roleId });
        }

        await _uow.SaveAsync(ct);
    }
}