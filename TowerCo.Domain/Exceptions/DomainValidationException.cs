namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when input breaks an entity invariant. Mapped to HTTP 400.</summary>
public sealed class DomainValidationException(string message) : DomainException(message);
