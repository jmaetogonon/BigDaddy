namespace BigDaddy.Domain.Users;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserTeam> UserTeams { get; set; } = [];
    public ICollection<TeamRole> TeamRoles { get; set; } = [];
}