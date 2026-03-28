using BigDaddy.Application.DTOs.Auth;

namespace BigDaddy.Application.Contracts.Persistence.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
    Task LogoutAsync(string jti, int userId, DateTime tokenExpiry, CancellationToken ct = default);
    Task<bool> IsTokenInvalidatedAsync(string jti, CancellationToken ct = default);
}
