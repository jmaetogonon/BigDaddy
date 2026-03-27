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
        base.OnModelCreating(modelBuilder);

        // ── User ────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Username).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.MobileNumber).HasMaxLength(20);
        });

        // ── Role ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => r.Name).IsUnique();
            e.Property(r => r.Name).HasMaxLength(100).IsRequired();
            e.Property(r => r.Description).HasMaxLength(500);
        });

        // ── Permission ──────────────────────────────────────────────────────
        modelBuilder.Entity<Permission>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Code).IsUnique();
            e.Property(p => p.Module).HasMaxLength(100).IsRequired();
            e.Property(p => p.Screen).HasMaxLength(100).IsRequired();
            e.Property(p => p.Action).HasMaxLength(100).IsRequired();
            e.Property(p => p.Code).HasMaxLength(200).IsRequired();
        });

        // ── Team ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Team>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasIndex(t => t.Name).IsUnique();
            e.Property(t => t.Name).HasMaxLength(100).IsRequired();
        });

        // ── UserRole (composite PK) ──────────────────────────────────────────
        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasKey(ur => new { ur.UserId, ur.RoleId });
            e.HasOne(ur => ur.User)
             .WithMany(u => u.UserRoles)
             .HasForeignKey(ur => ur.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ur => ur.Role)
             .WithMany(r => r.UserRoles)
             .HasForeignKey(ur => ur.RoleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── UserTeam (composite PK) ──────────────────────────────────────────
        modelBuilder.Entity<UserTeam>(e =>
        {
            e.HasKey(ut => new { ut.UserId, ut.TeamId });
            e.HasOne(ut => ut.User)
             .WithMany(u => u.UserTeams)
             .HasForeignKey(ut => ut.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(ut => ut.Team)
             .WithMany(t => t.UserTeams)
             .HasForeignKey(ut => ut.TeamId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── RolePermission (composite PK) ────────────────────────────────────
        modelBuilder.Entity<RolePermission>(e =>
        {
            e.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            e.HasOne(rp => rp.Role)
             .WithMany(r => r.RolePermissions)
             .HasForeignKey(rp => rp.RoleId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(rp => rp.Permission)
             .WithMany(p => p.RolePermissions)
             .HasForeignKey(rp => rp.PermissionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── TeamRole (composite PK) ──────────────────────────────────────────
        modelBuilder.Entity<TeamRole>(e =>
        {
            e.HasKey(tr => new { tr.TeamId, tr.RoleId });
            e.HasOne(tr => tr.Team)
             .WithMany(t => t.TeamRoles)
             .HasForeignKey(tr => tr.TeamId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(tr => tr.Role)
             .WithMany(r => r.TeamRoles)
             .HasForeignKey(tr => tr.RoleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── InvalidatedToken ─────────────────────────────────────────────────
        modelBuilder.Entity<InvalidatedToken>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasIndex(t => t.Jti).IsUnique();
            e.Property(t => t.Jti).HasMaxLength(200).IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
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