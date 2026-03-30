using BigDaddy.Application.Features.Users.DTOs;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Features.Users.Mappers;

public static class UserMappingExtensions
{
    public static UserDetailDto ToDetailDto(this User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FullName = $"{user.FirstName} {user.LastName}",
        Username = user.Username,
        Email = user.Email,
        MobileNumber = user.MobileNumber,
        IsActive = user.IsActive,
        IsLocked = user.IsLocked,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        Roles = user.UserRoles.Select(ur => new RoleAssignmentDto
        {
            RoleId = ur.RoleId,
            RoleName = ur.Role.Name,
            AssignedAt = ur.AssignedAt
        }).ToList(),
        Teams = user.UserTeams.Select(ut => new TeamAssignmentDto
        {
            TeamId = ut.TeamId,
            TeamName = ut.Team.Name,
            JoinedAt = ut.JoinedAt
        }).ToList()
    };

    public static UserListItemDto ToListItemDto(this User user) => new()
    {
        Id = user.Id,
        FullName = $"{user.FirstName} {user.LastName}",
        Username = user.Username,
        Email = user.Email,
        MobileNumber = user.MobileNumber,
        IsActive = user.IsActive,
        IsLocked = user.IsLocked,
        Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
        Teams = user.UserTeams.Select(ut => ut.Team.Name).ToList(),
        CreatedAt = user.CreatedAt
    };
}