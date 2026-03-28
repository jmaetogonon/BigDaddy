namespace BigDaddy.Application.Contracts.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepo Users { get; }
    IAuthRepo  Auth { get; }

    Task<int> SaveAsync(CancellationToken ct = default);
}