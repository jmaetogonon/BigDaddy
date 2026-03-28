using BigDaddy.Application.Contracts.Repositories;

namespace BigDaddy.Api.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<TokenCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var deleted = await uow.Auth.PurgeExpiredTokensAsync(stoppingToken);

            if (deleted > 0)
                _logger.LogInformation(
                    "Token cleanup: purged {Count} expired tokens.", deleted);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}