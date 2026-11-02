using TowerCo.Ingestion.Contracts;

namespace TowerCo.Ingestion.Processing;

public interface ISensorReadingParser
{
    SensorReadingMessage Parse(string json);
}
