using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using TowerCo.Domain.Exceptions;
using TowerCo.Ingestion.Exceptions;
using TowerCo.Ingestion.Processing;

namespace TowerCo.Ingestion.Functions;

/// <summary>POST /api/telemetry: lets the demo simulator (or Postman) push a reading without IoT Hub.</summary>
public sealed class HttpIngestFunction(ISensorReadingParser parser, ISensorReadingProcessor processor)
{
    [Function(IngestionConstants.HttpFunctionName)]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = IngestionConstants.TelemetryRoute)] HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(request.Body);
            var message = parser.Parse(await reader.ReadToEndAsync(cancellationToken));
            return new OkObjectResult(await processor.ProcessAsync(message, cancellationToken));
        }
        catch (Exception ex) when (ex is IngestionException or DomainException)
        {
            return new BadRequestObjectResult(new ProblemDetails { Title = IngestionConstants.InvalidPayloadTitle, Detail = ex.Message });
        }
    }
}
