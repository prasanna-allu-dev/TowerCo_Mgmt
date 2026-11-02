namespace TowerCo.Ingestion.Exceptions;

/// <summary>Root of expected ingestion failures. HTTP callers get 400; IoT Hub messages are logged and skipped.</summary>
public abstract class IngestionException(string message) : Exception(message);
