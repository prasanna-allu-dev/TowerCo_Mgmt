namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when decommissioning a site that still has active or future tenancies.</summary>
public sealed class SiteHasActiveTenantsException(string siteCode, int tenancies)
    : BusinessRuleException($"Site '{siteCode}' still has {tenancies} active or future tenancies; end them before decommissioning.");
