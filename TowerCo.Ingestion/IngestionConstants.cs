namespace TowerCo.Ingestion;

/// <summary>Names shared by function bindings, configuration and routes.</summary>
public static class IngestionConstants
{
    public const string HttpFunctionName = "IngestReading";
    public const string IoTHubFunctionName = "IngestFromIoTHub";
    public const string TelemetryRoute = "telemetry";
    public const string IoTHubConnectionSetting = "IoTHubConnection";
    public const string IoTHubEventHubNameExpression = "%IoTHubEventHubName%";
    public const string AlarmThresholdsSection = "AlarmThresholds";
    public const string InvalidPayloadTitle = "Invalid sensor reading";
}
