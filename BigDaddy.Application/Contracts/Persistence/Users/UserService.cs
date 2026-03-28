using BigDaddy.Application.Common;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.DTOs.Users;
using BigDaddy.Application.Mappers.Users;
using BigDaddy.Domain.Exceptions;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Contracts.Persistence.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;

    public UserService(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<UserListItemDto>> GetUsersAsync(
        UserQueryDto query, CancellationToken ct = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, total) = await _uow.Users.GetPagedAsync(query, ct);

        return new PagedResult<UserListItemDto>
        {
            Items = items.Select(UserMapper.ToListItem),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        return user is null ? null : UserMapper.ToDetail(user);
    }

    public async Task<UserDetailDto> CreateUserAsync(
        CreateUserDto dto, CancellationToken ct = default)
    {
        if (await _uow.Users.ExistsByEmailAsync(dto.Email, ct: ct))
            throw new ConflictException("A user with this email already exists.");

        if (await _uow.Users.ExistsByUsernameAsync(dto.Username, ct: ct))
            throw new ConflictException("A user with this username already exists.");

        await ValidateRoleIds(dto.RoleIds, ct);
        await ValidateTeamIds(dto.TeamIds, ct);

        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Username = dto.Username.Trim().ToLower(),
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            MobileNumber = dto.MobileNumber?.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _uow.Users.Add(user);

        _uow.Users.AddUserRoles(dto.RoleIds.Distinct()
            .Select(id => new UserRole { UserId = user.Id, RoleId = id }));
         
        _uow.Users.AddUserTeams(dto.TeamIds.Distinct()
            .Select(id => new UserTeam { UserId = user.Id, TeamId = id }));

        await _uow.SaveAsync(ct);

        return UserMapper.ToDetail((await _uow.Users.GetByIdAsync(user.Id, ct))!);
    }

    public async Task<UserDetailDto> UpdateUserAsync(
        int id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(id, ct);

        if (await _uow.Users.ExistsByEmailAsync(dto.Email, excludeId: id, ct: ct))
            throw new ConflictException("A user with this email already exists.");

        if (await _uow.Users.ExistsByUsernameAsync(dto.Username, excludeId: id, ct: ct))
            throw new ConflictException("A user with this username already exists.");

        await ValidateRoleIds(dto.RoleIds, ct);
        await ValidateTeamIds(dto.TeamIds, ct);

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Username = dto.Username.Trim().ToLower();
        user.Email = dto.Email.Trim().ToLower();
        user.MobileNumber = dto.MobileNumber?.Trim();
        user.IsActive = dto.IsActive;
        user.IsLocked = dto.IsLocked;
        user.UpdatedAt = DateTime.UtcNow;

        _uow.Users.RemoveUserRoles(user.UserRoles);
        _uow.Users.AddUserRoles(dto.RoleIds.Distinct()
            .Select(rid => new UserRole { UserId = user.Id, RoleId = rid }));

        _uow.Users.RemoveUserTeams(user.UserTeams);
        _uow.Users.AddUserTeams(dto.TeamIds.Distinct()
            .Select(tid => new UserTeam { UserId = user.Id, TeamId = tid }));

        await _uow.SaveAsync(ct);

        return UserMapper.ToDetail((await _uow.Users.GetByIdAsync(user.Id, ct))!);
    }

    public async Task DeleteUserAsync(int id, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(id, ct);
        _uow.Users.Remove(user);
        await _uow.SaveAsync(ct);
    }

    public async Task ActivateUserAsync(int id, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(id, ct);
        user.IsActive = true; user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }

    public async Task DeactivateUserAsync(int id, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(id, ct);
        user.IsActive = false; user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }

    public async Task LockUserAsync(int id, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(id, ct);
        user.IsLocked = true; user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }

    public async Task UnlockUserAsync(int id, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(id, ct);
        user.IsLocked = false; user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }

    public async Task ChangePasswordAsync(
        int userId, ChangePasswordDto dto, CancellationToken ct = default)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new ConflictException("New password and confirmation do not match.");

        var user = await GetTrackedOrThrowAsync(userId, ct);

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new ForbiddenException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }

    public async Task ResetPasswordAsync(
        int userId, ResetPasswordDto dto, CancellationToken ct = default)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new ConflictException("New password and confirmation do not match.");

        var user = await GetTrackedOrThrowAsync(userId, ct);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.IsLocked = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveAsync(ct);
    }

    public async Task AssignRolesAsync(
        int userId, List<int> roleIds, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(userId, ct);
        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();

        foreach (var roleId in roleIds.Distinct().Where(r => !existingRoleIds.Contains(r)))
        {
            if (!await _uow.Users.RoleExistsAsync(roleId, ct))
                throw new NotFoundException("Role", roleId);

            _uow.Users.AddUserRole(new UserRole { UserId = userId, RoleId = roleId });
        }

        await _uow.SaveAsync(ct);
    }

    public async Task AssignTeamsAsync(
        int userId, List<int> teamIds, CancellationToken ct = default)
    {
        var user = await GetTrackedOrThrowAsync(userId, ct);
        var existingTeamIds = user.UserTeams.Select(ut => ut.TeamId).ToHashSet();

        foreach (var teamId in teamIds.Distinct().Where(t => !existingTeamIds.Contains(t)))
        {
            if (!await _uow.Users.TeamExistsAsync(teamId, ct))
                throw new NotFoundException("Team", teamId);

            _uow.Users.AddUserTeam(new UserTeam { UserId = userId, TeamId = teamId });
        }

        await _uow.SaveAsync(ct);
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private async Task<User> GetTrackedOrThrowAsync(int id, CancellationToken ct)
        => await _uow.Users.GetByIdTrackedAsync(id, ct)
           ?? throw new NotFoundException("User", id);

    private async Task ValidateRoleIds(List<int> roleIds, CancellationToken ct)
    {
        if (roleIds.Count == 0) return;
        var found = await _uow.Users.CountRolesByIdsAsync(roleIds, ct);
        if (found != roleIds.Distinct().Count())
            throw new NotFoundException("One or more role IDs are invalid.");
    }

    private async Task ValidateTeamIds(List<int> teamIds, CancellationToken ct)
    {
        if (teamIds.Count == 0) return;
        var found = await _uow.Users.CountTeamsByIdsAsync(teamIds, ct);
        if (found != teamIds.Distinct().Count())
            throw new NotFoundException("One or more team IDs are invalid.");
    }
}