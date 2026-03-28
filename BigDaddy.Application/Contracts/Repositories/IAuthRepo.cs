using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Contracts.Repositories;

public interface IAuthRepo 
{
    Task<User?> GetUserForLoginAsync(string email, CancellationToken ct = default);
    Task<bool> IsTokenInvalidatedAsync(string jti, CancellationToken ct = default);
    void AddInvalidatedToken(InvalidatedToken token);
    Task<int> PurgeExpiredTokensAsync(CancellationToken ct = default);
}
