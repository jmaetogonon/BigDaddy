using BigDaddy.Application.Features.Users.Queries.GetUsers;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Contracts.Repositories;

public interface IUserRepo
{
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
           GetUsersQuery query, CancellationToken ct = default);

    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByIdTrackedAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null, CancellationToken ct = default);
    Task<int> CountRolesByIdsAsync(IEnumerable<int> roleIds, CancellationToken ct = default);
    Task<int> CountTeamsByIdsAsync(IEnumerable<int> teamIds, CancellationToken ct = default);
    Task<bool> RoleExistsAsync(int roleId, CancellationToken ct = default);
    Task<bool> TeamExistsAsync(int teamId, CancellationToken ct = default);

    void Add(User user);
    void Remove(User user);
    void AddUserRole(UserRole userRole);
    void AddUserTeam(UserTeam userTeam);
    void AddUserRoles(IEnumerable<UserRole> userRoles);
    void AddUserTeams(IEnumerable<UserTeam> userTeams);
    void RemoveUserRoles(IEnumerable<UserRole> userRoles);
    void RemoveUserTeams(IEnumerable<UserTeam> userTeams);
}
