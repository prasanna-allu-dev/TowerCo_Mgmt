namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when activating a site that has no tower to lease.</summary>
public sealed class SiteHasNoTowerException(string siteCode)
    : BusinessRuleException($"Site '{siteCode}' has no tower and cannot be made Active.");
