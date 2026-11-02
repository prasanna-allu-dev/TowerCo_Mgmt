namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a work order moves to Assigned without an engineer. Mapped to HTTP 400.</summary>
public sealed class WorkOrderAssigneeRequiredException()
    : DomainException("Select an engineer before assigning.");
