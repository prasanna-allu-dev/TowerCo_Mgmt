using TowerCo.Domain.Enums;

namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a work order status change is not allowed by the workflow.</summary>
public sealed class InvalidWorkOrderTransitionException(WorkOrderStatus from, WorkOrderStatus to)
    : BusinessRuleException($"Work order cannot move from {from} to {to}.");
