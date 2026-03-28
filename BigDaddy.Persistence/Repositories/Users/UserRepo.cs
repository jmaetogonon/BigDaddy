using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.DTOs.Users;
using BigDaddy.Domain.Users;
using BigDaddy.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BigDaddy.Persistence.Repositories.Users;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _db;

    public UserRepo(AppDbContext db) => _db = db;

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        UserQueryDto query, CancellationToken ct = default)
    {
        var q = _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserTeams).ThenInclude(ut => ut.Team)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term) ||
                u.Username.ToLower().Contains(term));
        }

        if (query.IsActive.HasValue) q = q.Where(u => u.IsActive == query.IsActive.Value);
        if (query.RoleId.HasValue) q = q.Where(u => u.UserRoles.Any(ur => ur.RoleId == query.RoleId.Value));
        if (query.TeamId.HasValue) q = q.Where(u => u.UserTeams.Any(ut => ut.TeamId == query.TeamId.Value));

        var asc = query.SortDir.ToLower() != "desc";
        q = query.SortBy.ToLower() switch
        {
            "firstname" => asc ? q.OrderBy(u => u.FirstName) : q.OrderByDescending(u => u.FirstName),
            "lastname" => asc ? q.OrderBy(u => u.LastName) : q.OrderByDescending(u => u.LastName),
            "email" => asc ? q.OrderBy(u => u.Email) : q.OrderByDescending(u => u.Email),
            "username" => asc ? q.OrderBy(u => u.Username) : q.OrderByDescending(u => u.Username),
            _ => asc ? q.OrderBy(u => u.CreatedAt) : q.OrderByDescending(u => u.CreatedAt)
        };

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Users
              .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
              .Include(u => u.UserTeams).ThenInclude(ut => ut.Team)
              .AsNoTracking()
              .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByIdTrackedAsync(int id, CancellationToken ct = default)
        => _db.Users
              .Include(u => u.UserRoles)
              .Include(u => u.UserTeams)
              .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => _db.Users.AnyAsync(u =>
              u.Email == email.Trim().ToLower() &&
              (excludeId == null || u.Id != excludeId), ct);

    public Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null, CancellationToken ct = default)
        => _db.Users.AnyAsync(u =>
              u.Username == username.Trim().ToLower() &&
              (excludeId == null || u.Id != excludeId), ct);

    public Task<int> CountRolesByIdsAsync(IEnumerable<int> roleIds, CancellationToken ct = default)
        => _db.Roles.CountAsync(r => roleIds.Contains(r.Id), ct);

    public Task<int> CountTeamsByIdsAsync(IEnumerable<int> teamIds, CancellationToken ct = default)
        => _db.Teams.CountAsync(t => teamIds.Contains(t.Id), ct);

    public Task<bool> RoleExistsAsync(int roleId, CancellationToken ct = default)
        => _db.Roles.AnyAsync(r => r.Id == roleId, ct);

    public Task<bool> TeamExistsAsync(int teamId, CancellationToken ct = default)
        => _db.Teams.AnyAsync(t => t.Id == teamId, ct);

    public void Add(User user) => _db.Users.Add(user);
    public void Remove(User user) => _db.Users.Remove(user);

    public void AddUserRole(UserRole ur) => _db.UserRoles.Add(ur);
    public void AddUserTeam(UserTeam ut) => _db.UserTeams.Add(ut);
    public void AddUserRoles(IEnumerable<UserRole> urs) => _db.UserRoles.AddRange(urs);
    public void AddUserTeams(IEnumerable<UserTeam> uts) => _db.UserTeams.AddRange(uts);
    public void RemoveUserRoles(IEnumerable<UserRole> urs) => _db.UserRoles.RemoveRange(urs);
    public void RemoveUserTeams(IEnumerable<UserTeam> uts) => _db.UserTeams.RemoveRange(uts);
}