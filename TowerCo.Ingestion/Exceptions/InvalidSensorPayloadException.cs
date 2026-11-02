namespace TowerCo.Ingestion.Exceptions;

/// <summary>Raised when a message body is not a readable sensor reading.</summary>
public sealed class InvalidSensorPayloadException(string message) : IngestionException(message);
