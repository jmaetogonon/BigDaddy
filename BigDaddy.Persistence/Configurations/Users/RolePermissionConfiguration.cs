using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigDaddy.Persistence.Configurations.Users;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        builder.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions)
               .HasForeignKey(rp => rp.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rp => rp.Permission).WithMany(p => p.RolePermissions)
               .HasForeignKey(rp => rp.PermissionId).OnDelete(DeleteBehavior.Cascade);
    }
}
