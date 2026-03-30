using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Features.Users.DTOs;

namespace BigDaddy.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IQuery<UserDetailDto?>
{
    public int Id { get; set; }
}
