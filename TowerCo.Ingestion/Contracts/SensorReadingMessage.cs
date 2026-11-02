using TowerCo.Domain.Monitoring;

namespace TowerCo.Ingestion.Contracts;

/// <summary>Wire format sent by a site gateway (via IoT Hub) or by the demo simulator (via HTTP).</summary>
public sealed record SensorReadingMessage(string SiteCode, SensorType SensorType, decimal Value, DateTime RecordedAtUtc);
