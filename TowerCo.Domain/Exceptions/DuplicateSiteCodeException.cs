namespace TowerCo.Domain.Exceptions;

/// <summary>Raised when registering a site whose code is already in use.</summary>
public sealed class DuplicateSiteCodeException(string siteCode)
    : BusinessRuleException($"Site code '{siteCode}' already exists.");
