using System.Text.Json;
using System.Text.Json.Serialization;
using TowerCo.Ingestion.Contracts;
using TowerCo.Ingestion.Exceptions;

namespace TowerCo.Ingestion.Processing;

/// <summary>Reads the JSON wire format (camelCase, enum names as text).</summary>
public sealed class SensorReadingParser : ISensorReadingParser
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public SensorReadingMessage Parse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<SensorReadingMessage>(json, SerializerOptions)
                ?? throw new InvalidSensorPayloadException("Message body is empty.");
        }
        catch (JsonException ex)
        {
            throw new InvalidSensorPayloadException($"Message body is not a valid sensor reading: {ex.Message}");
        }
    }
}
