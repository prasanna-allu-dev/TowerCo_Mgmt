namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a requested entity does not exist.</summary>
public sealed class NotFoundException(string entityName, object key)
    : DomainException($"{entityName} '{key}' was not found.");
