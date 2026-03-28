namespace BigDaddy.Domain.Users;

public class UserTeam
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Team Team { get; set; } = null!;
}