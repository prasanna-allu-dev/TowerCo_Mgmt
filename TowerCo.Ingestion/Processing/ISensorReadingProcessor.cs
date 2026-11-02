using TowerCo.Ingestion.Contracts;

namespace TowerCo.Ingestion.Processing;

public interface ISensorReadingProcessor
{
    Task<ReadingOutcome> ProcessAsync(SensorReadingMessage message, CancellationToken cancellationToken);
}
