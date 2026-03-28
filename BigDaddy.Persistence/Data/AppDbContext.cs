using BigDaddy.Domain.Common;
using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BigDaddy.Persistence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserTeam> UserTeams => Set<UserTeam>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<TeamRole> TeamRoles => Set<TeamRole>();
    public DbSet<InvalidatedToken> InvalidatedTokens => Set<InvalidatedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<IAuditableEntity>()
            .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
        {
            entry.Entity.UpdatedAt = DateTime.Now;
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}