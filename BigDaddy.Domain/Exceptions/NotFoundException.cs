namespace BigDaddy.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object key)
        : base($"{entity} with identifier '{key}' was not found.") { }

    public NotFoundException(string message) : base(message) { }
}
