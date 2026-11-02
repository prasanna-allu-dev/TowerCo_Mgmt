using System.Text.Json;
using System.Text.Json.Serialization;
using TowerCo.Application.Dashboard;

namespace TowerCo.Tests.Application;

public class DashboardAlertSerialisationTests
{
    [Theory]
    [InlineData(AlertSeverity.Critical, "critical")]
    [InlineData(AlertSeverity.Warning, "warning")]
    public void Serialize_WithGlobalEnumConverter_WritesCamelCaseSeverity(AlertSeverity severity, string expected)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var alert = new DashboardAlertDto("full-1", severity, "Tower full", "detail", 1);

        var json = JsonSerializer.Serialize(alert, options);

        Assert.Contains($"\"severity\":\"{expected}\"", json);
    }
}
