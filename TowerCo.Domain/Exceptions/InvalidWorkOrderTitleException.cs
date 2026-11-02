using TowerCo.Domain.Common;

namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a work order title is blank or too short. Mapped to HTTP 400.</summary>
public sealed class InvalidWorkOrderTitleException()
    : DomainException($"Title must be at least {DomainConstants.MinWorkOrderTitleLength} characters.");
