namespace TowerCo.Domain.Exceptions;

/// <summary>Base for business rule violations (for example tower capacity exceeded). Mapped to HTTP 409.</summary>
public abstract class BusinessRuleException(string message) : DomainException(message);
