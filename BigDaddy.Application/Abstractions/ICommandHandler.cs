namespace BigDaddy.Application.Abstractions;

/// <summary>Handler for commands that return a value (e.g. Create returns the created DTO).</summary>
public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct = default);
}

/// <summary>Handler for commands that return nothing (e.g. Delete, Activate).</summary>
public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken ct = default);
}
