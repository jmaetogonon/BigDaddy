using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigDaddy.Persistence.Configurations.Users;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Code).IsUnique();
        builder.Property(p => p.Module).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Screen).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Action).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(200).IsRequired();
    }
}
