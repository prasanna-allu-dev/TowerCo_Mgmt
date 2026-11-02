using TowerCo.Domain.Monitoring;

namespace TowerCo.Ingestion.Contracts;

/// <summary>Alarm raised when a reading breaches its threshold. Target queue: Service Bus "tower-alarms".</summary>
public sealed record SensorAlarm(SensorReading Reading, AlarmSeverity Severity, AlarmThreshold Threshold);
