using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigDaddy.Persistence.Data.Configurations.Users;

public class TeamRoleConfiguration : IEntityTypeConfiguration<TeamRole>
{
    public void Configure(EntityTypeBuilder<TeamRole> builder)
    {
        builder.HasKey(tr => new { tr.TeamId, tr.RoleId });
        builder.HasOne(tr => tr.Team).WithMany(t => t.TeamRoles)
               .HasForeignKey(tr => tr.TeamId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tr => tr.Role).WithMany(r => r.TeamRoles)
               .HasForeignKey(tr => tr.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}
