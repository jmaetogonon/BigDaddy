using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Identity;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.Features.Auth.DTOs;
using BigDaddy.Domain.Exceptions;

namespace BigDaddy.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUnitOfWork uow, ITokenService tokenService)
    {
        _uow = uow;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> HandleAsync(
        LoginCommand command, CancellationToken ct = default)
    {
        // Load user with full role/permission/team graph in one query
        var user = await _uow.Auth.GetUserForLoginAsync(command.Email, ct);

        // Identical error for not-found vs wrong password — prevents user enumeration
        if (user is null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            throw new ForbiddenException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("Account is inactive.");

        if (user.IsLocked)
            throw new ForbiddenException("Account is locked.");

        // Collect roles — direct assignments + team-inherited
        var roleNames = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToHashSet();

        foreach (var ut in user.UserTeams.Where(ut => ut.Team.IsActive))
            foreach (var tr in ut.Team.TeamRoles.Where(tr => tr.Role.IsActive))
                roleNames.Add(tr.Role.Name);

        // Collect permissions — direct roles + team-inherited roles
        var permCodes = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToHashSet();

        foreach (var ut in user.UserTeams.Where(ut => ut.Team.IsActive))
            foreach (var tr in ut.Team.TeamRoles.Where(tr => tr.Role.IsActive))
                foreach (var rp in tr.Role.RolePermissions)
                    permCodes.Add(rp.Permission.Code);

        var (token, jti, expiresAt) = _tokenService.GenerateToken(
            user, roleNames, permCodes);

        return new LoginResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            User = new UserProfileDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Username = user.Username,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Roles = roleNames.ToList(),
                Teams = user.UserTeams.Select(ut => ut.Team.Name).ToList(),
                Permissions = permCodes.ToList()
            }
        };
    }
}