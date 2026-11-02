using TowerCo.Domain.Monitoring;

namespace TowerCo.Ingestion.Exceptions;

/// <summary>Raised when no threshold is configured for a sensor type.</summary>
public sealed class MissingAlarmThresholdException(SensorType sensorType)
    : IngestionException($"No alarm threshold configured for sensor type '{sensorType}'.");
