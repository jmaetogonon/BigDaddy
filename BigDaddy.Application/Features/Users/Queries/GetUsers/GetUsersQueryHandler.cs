using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Common;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.Features.Users.DTOs;
using BigDaddy.Application.Features.Users.Mappers;

namespace BigDaddy.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler
    : IQueryHandler<GetUsersQuery, PagedResult<UserListItemDto>>
{
    private readonly IUnitOfWork _uow;

    public GetUsersQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<UserListItemDto>> HandleAsync(
        GetUsersQuery query, CancellationToken ct = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var (items, total) = await _uow.Users.GetPagedAsync(query, ct);

        return new PagedResult<UserListItemDto>
        {
            Items = items.Select(u => u.ToListItemDto()),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }
}