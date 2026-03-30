using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigDaddy.Persistence.Data.Configurations.Users;

public class InvalidatedTokenConfiguration : IEntityTypeConfiguration<InvalidatedToken>
{
    public void Configure(EntityTypeBuilder<InvalidatedToken> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Jti).IsUnique();
        builder.Property(t => t.Jti).HasMaxLength(200).IsRequired();
        builder.HasIndex(t => t.ExpiresAt);  // index for efficient cleanup queries
    }
}