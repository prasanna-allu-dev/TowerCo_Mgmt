namespace TowerCo_Mgmt.Server;

/// <summary>Route templates for every API endpoint. Single place to change a URL.</summary>
public static class ApiRoutes
{
    public const string Base = "api";
    public const string Sites = Base + "/sites";
    public const string Operators = Base + "/operators";
    public const string Towers = Base + "/towers";
    public const string Dashboard = Base + "/dashboard";
    public const string WorkOrders = Base + "/work-orders";
    public const string HealthSites = Base + "/health/sites";
    public const string BySiteId = "{siteId:int}";

    public const string ById = "{id:int}";
    public const string Tenancies = "tenancies";
    public const string TowerTenancies = "towers/{towerId:int}/tenancies";
    public const string Equipment = "equipment";
    public const string EquipmentTypes = "equipment-types";
    public const string WorkOrderStatus = "{id:int}/status";
}
