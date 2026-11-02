using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TowerCo.Domain.Monitoring;
using TowerCo.Ingestion;
using TowerCo.Ingestion.Contracts;
using TowerCo.Ingestion.Functions;
using TowerCo.Ingestion.Processing;
using static TowerCo.Tests.Ingestion.IngestionBuilders;

namespace TowerCo.Tests.Ingestion;

public class IngestionFunctionTests
{
    private readonly SensorReadingParser _parser = new();
    private readonly RecordingAlarmPublisher _publisher = new();

    [Fact]
    public async Task Http_ValidReading_ReturnsOkWithOutcome()
    {
        var result = await HttpFunction().RunAsync(Request(Json("ShelterTemperature", 40m)), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(AlarmSeverity.Warning, Assert.IsType<ReadingOutcome>(ok.Value).Severity);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{ "siteCode": "", "sensorType": "ShelterTemperature", "value": 1, "recordedAtUtc": "2026-11-02T09:00:00Z" }""")]
    public async Task Http_BadReading_ReturnsProblemDetails(string body)
    {
        var result = await HttpFunction().RunAsync(Request(body), CancellationToken.None);

        var problem = Assert.IsType<ProblemDetails>(Assert.IsType<BadRequestObjectResult>(result).Value);
        Assert.Equal(IngestionConstants.InvalidPayloadTitle, problem.Title);
    }

    [Fact]
    public async Task IoTHub_MixedBatch_ProcessesGoodAndLogsBad()
    {
        var logger = new RecordingLogger<IoTHubIngestFunction>();
        var function = new IoTHubIngestFunction(_parser, Processor(_publisher), logger);

        await function.RunAsync([Json("ShelterTemperature", 46m), "not json", Json("ShelterTemperature", 20m)], CancellationToken.None);

        Assert.Single(_publisher.Published);
        Assert.Equal(LogLevel.Error, Assert.Single(logger.Entries).Level);
    }

    private HttpIngestFunction HttpFunction() => new(_parser, Processor(_publisher));

    private static HttpRequest Request(string body)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        return context.Request;
    }
}
