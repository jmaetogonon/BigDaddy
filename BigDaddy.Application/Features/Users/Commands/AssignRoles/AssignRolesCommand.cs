using BigDaddy.Application.Abstractions;

namespace BigDaddy.Application.Features.Users.Commands.AssignRoles;

public class AssignRolesCommand : ICommand
{
    public int UserId { get; set; }
    public List<int> RoleIds { get; set; } = [];
}
