using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Application.Features.Users.DTOs;
using BigDaddy.Application.Features.Users.Mappers;

namespace BigDaddy.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler
    : IQueryHandler<GetUserByIdQuery, UserDetailDto?>
{
    private readonly IUnitOfWork _uow;

    public GetUserByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<UserDetailDto?> HandleAsync(
        GetUserByIdQuery query, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(query.Id, ct);
        return user?.ToDetailDto();
    }
}