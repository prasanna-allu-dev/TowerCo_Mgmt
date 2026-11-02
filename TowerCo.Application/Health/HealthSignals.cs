namespace TowerCo.Application.Health;

/// <summary>Signal names and reason templates shown in the UI.</summary>
public static class HealthSignals
{
    public const string Telemetry = "Telemetry";
    public const string Inspection = "Inspection";
    public const string WorkOrders = "Work orders";

    public const string NoTelemetry = "No telemetry reported";
    public const string TelemetryOk = "All sensors within limits";
    public const string SensorBreach = "{0} {1} ({2})";
    public const string NoInspection = "No inspection on record";
    public const string InspectionFailed = "Last inspection failed on {0:yyyy-MM-dd}";
    public const string InspectionAge = "Last inspected {0} months ago";
    public const string NoOpenWorkOrders = "No open work orders";
    public const string UrgentWorkOrders = "{0} open, {1} critical priority";
    public const string OpenWorkOrders = "{0} open work orders";
}
