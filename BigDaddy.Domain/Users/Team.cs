using BigDaddy.Domain.Common;

namespace BigDaddy.Domain.Users;

public class Team : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
 
    // Navigation
    public ICollection<UserTeam> UserTeams { get; set; } = [];
    public ICollection<TeamRole> TeamRoles { get; set; } = [];
}