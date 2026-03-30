using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.Features.Users.DTOs;
using BigDaddy.Application.Features.Users.Mappers;
using BigDaddy.Domain.Exceptions;
using BigDaddy.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace BigDaddy.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDetailDto>
{
    private readonly IUnitOfWork _uow;

    public CreateUserCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<UserDetailDto> HandleAsync(
        CreateUserCommand command, CancellationToken ct = default)
    {
        if (await _uow.Users.ExistsByEmailAsync(command.Email, ct: ct))
            throw new ConflictException("A user with this email already exists.");

        if (await _uow.Users.ExistsByUsernameAsync(command.Username, ct: ct))
            throw new ConflictException("A user with this username already exists.");

        await ValidateRoleIdsAsync(command.RoleIds, ct);
        await ValidateTeamIdsAsync(command.TeamIds, ct);

        var user = new User
        {
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            Username = command.Username.Trim().ToLower(),
            Email = command.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            MobileNumber = command.MobileNumber?.Trim(),
            IsActive = command.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _uow.Users.Add(user);

        _uow.Users.AddUserRoles(command.RoleIds.Distinct()
            .Select(id => new UserRole { UserId = user.Id, RoleId = id }));

        _uow.Users.AddUserTeams(command.TeamIds.Distinct()
            .Select(id => new UserTeam { UserId = user.Id, TeamId = id }));

        await _uow.SaveAsync(ct);

        return (await _uow.Users.GetByIdAsync(user.Id, ct))!.ToDetailDto();
    }

    private async Task ValidateRoleIdsAsync(List<int> roleIds, CancellationToken ct)
    {
        if (roleIds.Count == 0) return;
        var found = await _uow.Users.CountRolesByIdsAsync(roleIds, ct);
        if (found != roleIds.Distinct().Count())
            throw new NotFoundException("One or more role IDs are invalid.");
    }

    private async Task ValidateTeamIdsAsync(List<int> teamIds, CancellationToken ct)
    {
        if (teamIds.Count == 0) return;
        var found = await _uow.Users.CountTeamsByIdsAsync(teamIds, ct);
        if (found != teamIds.Distinct().Count())
            throw new NotFoundException("One or more team IDs are invalid.");
    }
}
