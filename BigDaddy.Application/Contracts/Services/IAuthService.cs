using BigDaddy.Application.Models.Auth;

namespace BigDaddy.Application.Contracts.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task LogoutAsync(string jti, int userId, DateTime tokenExpiry);

    Task<bool> IsTokenInvalidatedAsync(string jti);
}