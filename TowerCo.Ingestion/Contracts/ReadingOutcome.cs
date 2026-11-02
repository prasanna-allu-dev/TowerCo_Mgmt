using TowerCo.Domain.Monitoring;

namespace TowerCo.Ingestion.Contracts;

/// <summary>Result of processing one reading, returned to HTTP callers.</summary>
public sealed record ReadingOutcome(string SiteCode, SensorType SensorType, decimal Value, AlarmSeverity Severity);
