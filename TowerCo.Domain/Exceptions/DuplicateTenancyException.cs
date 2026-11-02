namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when an operator already holds an active tenancy on the tower (rule 2).</summary>
public sealed class DuplicateTenancyException(string operatorName, string siteCode)
    : BusinessRuleException($"Operator '{operatorName}' already has an active tenancy on site '{siteCode}'.");
