namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a second tower is added to a site (one tower per site).</summary>
public sealed class TowerAlreadyExistsException(string siteCode)
    : BusinessRuleException($"Site '{siteCode}' already has a tower.");
