namespace TowerCo.Application.Constants;

/// <summary>Query-string values shared with the client filters.</summary>
public static class FilterValues
{
    public const string OwnerTenant = "tenant";
    public const string OwnerShared = "shared";
    public const string TenancyActive = "active";
    public const string TenancyEnded = "ended";
    public const string WorkOrderCodeFormat = "WO-{0:D4}";
}
