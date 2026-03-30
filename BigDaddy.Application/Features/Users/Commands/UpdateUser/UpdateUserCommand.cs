using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Features.Users.DTOs;

namespace BigDaddy.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : ICommand<UserDetailDto>
{
    public int Id { get; set; }   // set by controller from route
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? MobileNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public List<int> RoleIds { get; set; } = [];
    public List<int> TeamIds { get; set; } = [];
}
