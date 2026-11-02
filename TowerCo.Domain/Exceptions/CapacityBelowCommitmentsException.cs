namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when tower capacity is reduced below the tenancies already committed.</summary>
public sealed class CapacityBelowCommitmentsException(string siteCode, int requested, int committed)
    : BusinessRuleException($"Tower at '{siteCode}' cannot drop to {requested} slots; {committed} are committed to tenants.");
