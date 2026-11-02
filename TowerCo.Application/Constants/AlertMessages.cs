namespace TowerCo.Application.Constants;

/// <summary>Dashboard alert ids and text templates.</summary>
public static class AlertMessages
{
    public const string FullId = "full-{0}";
    public const string FullTitle = "Tower full: {0}";
    public const string FullDetail = "{0} of {1} slots used. New tenant demand cannot be served.";
    public const string EmptyId = "empty-{0}";
    public const string EmptyTitle = "No tenants: {0}";
    public const string EmptyDetail = "{0} is active but earns no rent.";
    public const string DecommissionedId = "decom-{0}";
    public const string DecommissionedTitle = "Tenants on decommissioned site: {0}";
    public const string DecommissionedDetail = "{0} active tenancies need migration.";
    public const string RenewalId = "renewal-{0}";
    public const string RenewalTitle = "Tenancy ending: {0} on {1}";
    public const string RenewalDetail = "Ends {0:yyyy-MM-dd}, {1} days left.";
}
