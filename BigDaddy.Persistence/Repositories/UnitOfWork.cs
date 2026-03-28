using BigDaddy.Application.Contracts.Repositories;
using BigDaddy.Persistence.Data;
using BigDaddy.Persistence.Repositories.Auth;
using BigDaddy.Persistence.Repositories.Users;

namespace BigDaddy.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    private IUserRepo? _users;
    private IAuthRepo? _auth;

    public UnitOfWork(AppDbContext db) => _db = db;

    public IUserRepo Users => _users ??= new UserRepo(_db);
    public IAuthRepo Auth => _auth ??= new AuthRepo(_db);

    public Task<int> SaveAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public ValueTask DisposeAsync() => _db.DisposeAsync();
}