namespace BigDaddy.Application.DTOs.Users;

public class TeamAssignmentDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
