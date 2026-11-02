namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when ending, renewing or re-pricing a tenancy breaks its commercial terms.</summary>
public sealed class InvalidTenancyChangeException(string reason) : BusinessRuleException(reason);
