using BigDaddy.Application.Contracts.Identity;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.DTOs.Auth;
using BigDaddy.Domain.Exceptions;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Contracts.Persistence.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;

    public AuthService(IUnitOfWork uow, ITokenService tokenService)
    {
        _uow = uow;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken ct = default)
    {
        var user = await _uow.Auth.GetUserForLoginAsync(request.Email, ct);

        // Same error for not-found vs wrong password — prevents user enumeration
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new ForbiddenException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("Account is inactive.");

        if (user.IsLocked)
            throw new ForbiddenException("Account is locked.");

        var roleNames = CollectRoleNames(user);
        var permCodes = CollectPermissionCodes(user);

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
                Roles = roleNames,
                Teams = user.UserTeams.Select(ut => ut.Team.Name).ToList(),
                Permissions = permCodes
            }
        };
    }

    public async Task LogoutAsync(
        string jti,
        int userId,
        DateTime tokenExpiry,
        CancellationToken ct = default)
    {
        if (await _uow.Auth.IsTokenInvalidatedAsync(jti, ct))
            return; // idempotent

        _uow.Auth.AddInvalidatedToken(new InvalidatedToken
        {
            Jti = jti,
            UserId = userId,
            ExpiresAt = tokenExpiry,
            InvalidatedAt = DateTime.UtcNow
        });

        await _uow.SaveAsync(ct);
    }

    public Task<bool> IsTokenInvalidatedAsync(string jti, CancellationToken ct = default)
        => _uow.Auth.IsTokenInvalidatedAsync(jti, ct);

    // ── Private helpers ────────────────────────────────────────────────────

    private static List<string> CollectRoleNames(User user)
    {
        var names = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToHashSet();

        foreach (var ut in user.UserTeams.Where(ut => ut.Team.IsActive))
            foreach (var tr in ut.Team.TeamRoles.Where(tr => tr.Role.IsActive))
                names.Add(tr.Role.Name);

        return names.ToList();
    }

    private static List<string> CollectPermissionCodes(User user)
    {
        var codes = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToHashSet();

        foreach (var ut in user.UserTeams.Where(ut => ut.Team.IsActive))
            foreach (var tr in ut.Team.TeamRoles.Where(tr => tr.Role.IsActive))
                foreach (var rp in tr.Role.RolePermissions)
                    codes.Add(rp.Permission.Code);

        return codes.ToList();
    }
}