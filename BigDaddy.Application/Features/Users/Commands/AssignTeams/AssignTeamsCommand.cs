using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.AssignTeams;

public class AssignTeamsCommand : ICommand
{
    public int UserId { get; set; }
    public List<int> TeamIds { get; set; } = [];
}
