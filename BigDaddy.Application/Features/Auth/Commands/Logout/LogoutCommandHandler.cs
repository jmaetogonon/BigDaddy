
using BigDaddy.Application.Abstractions;
using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Domain.Users;

namespace BigDaddy.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IUnitOfWork _uow;

    public LogoutCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task HandleAsync(LogoutCommand command, CancellationToken ct = default)
    {
        // Idempotent — safe to call multiple times
        if (await _uow.Auth.IsTokenInvalidatedAsync(command.Jti, ct))
            return;

        _uow.Auth.AddInvalidatedToken(new InvalidatedToken
        {
            Jti = command.Jti,
            UserId = command.UserId,
            ExpiresAt = command.TokenExpiry,
            InvalidatedAt = DateTime.UtcNow
        });

        await _uow.SaveAsync(ct);
    }
}