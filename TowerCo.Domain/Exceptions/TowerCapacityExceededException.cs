namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a tenancy is added to a tower with no free slots (rule 1).</summary>
public sealed class TowerCapacityExceededException(string siteCode, int maxTenants)
    : BusinessRuleException($"Tower at site '{siteCode}' is full ({maxTenants} of {maxTenants} slots used).");
