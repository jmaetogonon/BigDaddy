namespace BigDaddy.Application.Abstractions;

/// <summary>Marker interface for commands that return a value.</summary>
public interface ICommand<TResponse> { }

/// <summary>Marker interface for commands that return nothing.</summary>
public interface ICommand { }
