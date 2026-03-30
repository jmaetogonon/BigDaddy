using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Common;
using BigDaddy.Application.Features.Users.DTOs;

namespace BigDaddy.Application.Features.Users.Queries.GetUsers;


/// <summary>
/// Query parameters double as the query object.
/// Bind directly from [FromQuery] in the controller.
/// </summary>
public class GetUsersQuery : IQuery<PagedResult<UserListItemDto>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int? RoleId { get; set; }
    public int? TeamId { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDir { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
