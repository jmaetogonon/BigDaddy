using BigDaddy.Domain.Common;

namespace BigDaddy.Domain.Users;

public class TeamRole  
{
    public int TeamId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public Team Team { get; set; } = null!;
    public Role Role { get; set; } = null!;
}