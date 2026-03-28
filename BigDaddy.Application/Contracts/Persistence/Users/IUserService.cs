using BigDaddy.Application.Common;
using BigDaddy.Application.DTOs.Users;

namespace BigDaddy.Application.Contracts.Persistence.Users;

public interface IUserService
{
    Task<PagedResult<UserListItemDto>> GetUsersAsync(UserQueryDto query, CancellationToken ct = default);
    Task<UserDetailDto?> GetUserByIdAsync(int id, CancellationToken ct = default);
    Task<UserDetailDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<UserDetailDto> UpdateUserAsync(int id, UpdateUserDto dto, CancellationToken ct = default);
    Task DeleteUserAsync(int id, CancellationToken ct = default);
    Task ActivateUserAsync(int id, CancellationToken ct = default);
    Task DeactivateUserAsync(int id, CancellationToken ct = default);
    Task LockUserAsync(int id, CancellationToken ct = default);
    Task UnlockUserAsync(int id, CancellationToken ct = default);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken ct = default);
    Task ResetPasswordAsync(int userId, ResetPasswordDto dto, CancellationToken ct = default);
    Task AssignRolesAsync(int userId, List<int> roleIds, CancellationToken ct = default);
    Task AssignTeamsAsync(int userId, List<int> teamIds, CancellationToken ct = default);
}
