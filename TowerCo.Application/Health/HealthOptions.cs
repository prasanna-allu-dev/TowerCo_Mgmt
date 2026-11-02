using System.ComponentModel.DataAnnotations;
using TowerCo.Domain.Monitoring;

namespace TowerCo.Application.Health;

/// <summary>Bound from the "Health" section of appsettings.json.</summary>
public sealed class HealthOptions
{
    [Range(1, int.MaxValue)]
    public int TelemetryWindowDays { get; init; }

    [Range(1, int.MaxValue)]
    public int InspectionWarningMonths { get; init; }

    [Range(1, int.MaxValue)]
    public int InspectionCriticalMonths { get; init; }
}

/// <summary>Sensor limits per type, bound from "AlarmThresholds" (same shape the ingestion function uses).</summary>
public sealed class SensorThresholdOptions : Dictionary<SensorType, AlarmThreshold>;
