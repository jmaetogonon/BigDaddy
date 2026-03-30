using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.Features.Users.DTOs;
using BigDaddy.Application.Features.Users.Mappers;
using BigDaddy.Domain.Exceptions;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserDetailDto>
{
    private readonly IUnitOfWork _uow;

    public UpdateUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<UserDetailDto> HandleAsync(
        UpdateUserCommand command, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdTrackedAsync(command.Id, ct)
            ?? throw new NotFoundException("User", command.Id);

        if (await _uow.Users.ExistsByEmailAsync(command.Email, excludeId: command.Id, ct: ct))
            throw new ConflictException("A user with this email already exists.");

        if (await _uow.Users.ExistsByUsernameAsync(command.Username, excludeId: command.Id, ct: ct))
            throw new ConflictException("A user with this username already exists.");

        await ValidateRoleIdsAsync(command.RoleIds, ct);
        await ValidateTeamIdsAsync(command.TeamIds, ct);

        user.FirstName = command.FirstName.Trim();
        user.LastName = command.LastName.Trim();
        user.Username = command.Username.Trim().ToLower();
        user.Email = command.Email.Trim().ToLower();
        user.MobileNumber = command.MobileNumber?.Trim();
        user.IsActive = command.IsActive;
        user.IsLocked = command.IsLocked;
        user.UpdatedAt = DateTime.UtcNow;

        _uow.Users.RemoveUserRoles(user.UserRoles);
        _uow.Users.AddUserRoles(command.RoleIds.Distinct()
            .Select(id => new UserRole { UserId = user.Id, RoleId = id }));

        _uow.Users.RemoveUserTeams(user.UserTeams);
        _uow.Users.AddUserTeams(command.TeamIds.Distinct()
            .Select(id => new UserTeam { UserId = user.Id, TeamId = id }));

        await _uow.SaveAsync(ct);

        return (await _uow.Users.GetByIdAsync(user.Id, ct))!.ToDetailDto();
    }

    private async Task ValidateRoleIdsAsync(List<int> ids, CancellationToken ct)
    {
        if (ids.Count == 0) return;
        if (await _uow.Users.CountRolesByIdsAsync(ids, ct) != ids.Distinct().Count())
            throw new NotFoundException("One or more role IDs are invalid.");
    }

    private async Task ValidateTeamIdsAsync(List<int> ids, CancellationToken ct)
    {
        if (ids.Count == 0) return;
        if (await _uow.Users.CountTeamsByIdsAsync(ids, ct) != ids.Distinct().Count())
            throw new NotFoundException("One or more team IDs are invalid.");
    }
}