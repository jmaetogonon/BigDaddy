using Microsoft.Extensions.DependencyInjection;

namespace BigDaddy.Application.Abstractions;

/// <summary>
/// Replaces MediatR. Resolves the correct handler from DI and invokes it.
/// All handlers must be registered — use AddHandlersFromAssembly() in DI setup.
/// </summary>
public class Dispatcher
{
    private readonly IServiceProvider _sp;

    public Dispatcher(IServiceProvider sp) => _sp = sp;

    // ── Query dispatch ─────────────────────────────────────────────────────────
    public Task<TResponse> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken ct = default)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResponse));

        var handler = _sp.GetRequiredService(handlerType);

        return (Task<TResponse>)handlerType
            .GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.HandleAsync))!
            .Invoke(handler, [query, ct])!;
    }

    // ── Command dispatch (with return value) ───────────────────────────────────
    public Task<TResponse> CommandAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken ct = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResponse));

        var handler = _sp.GetRequiredService(handlerType);

        return (Task<TResponse>)handlerType
            .GetMethod(nameof(ICommandHandler<ICommand<TResponse>, TResponse>.HandleAsync))!
            .Invoke(handler, [command, ct])!;
    }

    // ── Command dispatch (no return value) ────────────────────────────────────
    public Task CommandAsync(
        ICommand command,
        CancellationToken ct = default)
    {
        var handlerType = typeof(ICommandHandler<>)
            .MakeGenericType(command.GetType());

        var handler = _sp.GetRequiredService(handlerType);

        return (Task)handlerType
            .GetMethod(nameof(ICommandHandler<ICommand>.HandleAsync))!
            .Invoke(handler, [command, ct])!;
    }
}