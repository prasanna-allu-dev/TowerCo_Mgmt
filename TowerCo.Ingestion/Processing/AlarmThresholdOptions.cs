using TowerCo.Domain.Monitoring;

namespace TowerCo.Ingestion.Processing;

/// <summary>Thresholds per sensor type, bound from the "AlarmThresholds" configuration section.</summary>
public sealed class AlarmThresholdOptions : Dictionary<SensorType, AlarmThreshold>;
