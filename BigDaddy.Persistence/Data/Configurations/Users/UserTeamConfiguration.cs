using BigDaddy.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigDaddy.Persistence.Data.Configurations.Users;

public class UserTeamConfiguration : IEntityTypeConfiguration<UserTeam>
{
    public void Configure(EntityTypeBuilder<UserTeam> builder)
    {
        builder.HasKey(ut => new { ut.UserId, ut.TeamId });
        builder.HasOne(ut => ut.User).WithMany(u => u.UserTeams)
               .HasForeignKey(ut => ut.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ut => ut.Team).WithMany(t => t.UserTeams)
               .HasForeignKey(ut => ut.TeamId).OnDelete(DeleteBehavior.Cascade);
    }
}
