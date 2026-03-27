using BigDaddy.Application.Contracts.Services;
using BigDaddy.Application.Models.Auth;
using BigDaddy.Domain.Users;
using BigDaddy.Persistence.Data;
using BigDaddy.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BigDaddy.Persistence.Services.Auth;

public class AuthService(AppDbContext db, JwtHelper jwt) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly JwtHelper _jwt = jwt;

    // ── LOGIN ─────────────────────────────────────────────────────────────────
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 1. Find user by email
        var user = await _db.Users
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
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower());

        // 2. Validate existence and password before checking lock/active
        //    (avoid user-enumeration via different error messages for locked vs. not found)
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        // 3. Check active
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is inactive.");

        // 4. Check locked
        if (user.IsLocked)
            throw new UnauthorizedAccessException("Account is locked.");

        // 5. Collect all roles — from direct role assignments
        var roleNames = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .ToHashSet();

        // 6. Also collect roles inherited from teams
        foreach (var ut in user.UserTeams.Where(ut => ut.Team.IsActive))
            foreach (var tr in ut.Team.TeamRoles.Where(tr => tr.Role.IsActive))
                roleNames.Add(tr.Role.Name);

        // 7. Collect all permissions — from direct roles
        var permCodes = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToHashSet();

        // 8. Also collect permissions from team-inherited roles
        foreach (var ut in user.UserTeams.Where(ut => ut.Team.IsActive))
            foreach (var tr in ut.Team.TeamRoles.Where(tr => tr.Role.IsActive))
                foreach (var rp in tr.Role.RolePermissions)
                    permCodes.Add(rp.Permission.Code);

        // 9. Generate JWT
        var (token, jti, expiresAt) = _jwt.GenerateToken(
            user,
            roleNames.ToList(),
            permCodes.ToList()
        );

        // 10. Build response
        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            User = new UserProfile
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

    // ── LOGOUT ────────────────────────────────────────────────────────────────
    public async Task LogoutAsync(string jti, int userId, DateTime tokenExpiry)
    {
        // Idempotent — ignore if already logged out
        var alreadyInvalidated = await _db.InvalidatedTokens
            .AnyAsync(t => t.Jti == jti);

        if (!alreadyInvalidated)
        {
            _db.InvalidatedTokens.Add(new InvalidatedToken
            {
                Jti = jti,
                UserId = userId,
                ExpiresAt = tokenExpiry,
                InvalidatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }
    }

    // ── TOKEN BLACKLIST CHECK ─────────────────────────────────────────────────
    public async Task<bool> IsTokenInvalidatedAsync(string jti)
    {
        return await _db.InvalidatedTokens.AnyAsync(t => t.Jti == jti);
    }
}