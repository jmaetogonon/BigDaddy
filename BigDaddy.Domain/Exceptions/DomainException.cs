namespace BigDaddy.Domain.Exceptions;

/// <summary>Base class for all domain-level exceptions.</summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
