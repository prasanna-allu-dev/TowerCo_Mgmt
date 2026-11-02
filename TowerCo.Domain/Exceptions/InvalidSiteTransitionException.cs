using TowerCo.Domain.Enums;

namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a site status change skips or reverses the lifecycle.</summary>
public sealed class InvalidSiteTransitionException(string siteCode, SiteStatus from, SiteStatus to)
    : BusinessRuleException($"Site '{siteCode}' cannot move from {from} to {to}.");
