using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Contracts.Identity;

public interface ITokenService
{
    (string Token, string Jti, DateTime ExpiresAt) GenerateToken(
        User user,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);
}