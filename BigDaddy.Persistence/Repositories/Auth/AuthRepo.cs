using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Users;
using BigDaddy.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BigDaddy.Persistence.Repositories.Auth;

public class AuthRepo : IAuthRepo
{
    private readonly AppDbContext _db;

    public AuthRepo(AppDbContext db) => _db = db;

    public Task<User?> GetUserForLoginAsync(string email, CancellationToken ct = default)
        => _db.Users
              .Include(u => u.UserRoles)
                  .ThenInclude(ur => ur.Role)
                      .ThenInclude(r => r.RolePermissions)
                          .ThenInclude(rp => rp.Permission)
              .Include(u => u.UserTeams)
                  .ThenInclude(ut => ut.Team)
                      .ThenInclude(t => t.TeamRoles)
                          .ThenInclude(tr => tr.Role)
                              .ThenInclude(r => r.RolePermissions)
                                  .ThenInclude(rp => rp.Permission)
              .FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower(), ct);

    public Task<bool> IsTokenInvalidatedAsync(string jti, CancellationToken ct = default)
        => _db.InvalidatedTokens.AnyAsync(t => t.Jti == jti, ct);

    public void AddInvalidatedToken(InvalidatedToken token)
        => _db.InvalidatedTokens.Add(token);

    public Task<int> PurgeExpiredTokensAsync(CancellationToken ct = default)
        => _db.InvalidatedTokens
              .Where(t => t.ExpiresAt < DateTime.UtcNow)
              .ExecuteDeleteAsync(ct);
}