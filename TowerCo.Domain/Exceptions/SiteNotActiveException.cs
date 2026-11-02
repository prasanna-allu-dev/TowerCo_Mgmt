using TowerCo.Domain.Enums;

namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when a tenancy is added to a site that is not Active (rule 3).</summary>
public sealed class SiteNotActiveException(string siteCode, SiteStatus status)
    : BusinessRuleException($"Site '{siteCode}' is {status}; tenants can be added only to Active sites.");
