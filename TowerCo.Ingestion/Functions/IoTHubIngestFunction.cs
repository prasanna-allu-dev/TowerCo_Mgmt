using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TowerCo.Domain.Exceptions;
using TowerCo.Ingestion.Exceptions;
using TowerCo.Ingestion.Processing;

namespace TowerCo.Ingestion.Functions;

/// <summary>Reads gateway telemetry from the IoT Hub built-in (Event Hub compatible) endpoint.
/// A bad message is logged and skipped so one faulty sensor cannot block the batch.</summary>
public sealed class IoTHubIngestFunction(
    ISensorReadingParser parser, ISensorReadingProcessor processor, ILogger<IoTHubIngestFunction> logger)
{
    [Function(IngestionConstants.IoTHubFunctionName)]
    public async Task RunAsync(
        [EventHubTrigger(IngestionConstants.IoTHubEventHubNameExpression, Connection = IngestionConstants.IoTHubConnectionSetting)] string[] messages,
        CancellationToken cancellationToken)
    {
        foreach (var body in messages)
        {
            await ProcessOneAsync(body, cancellationToken);
        }
    }

    private async Task ProcessOneAsync(string body, CancellationToken cancellationToken)
    {
        try
        {
            await processor.ProcessAsync(parser.Parse(body), cancellationToken);
        }
        catch (Exception ex) when (ex is IngestionException or DomainException)
        {
            logger.LogError(ex, "Skipped unreadable sensor message: {Reason}", ex.Message);
        }
    }
}
