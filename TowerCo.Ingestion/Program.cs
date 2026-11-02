using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using TowerCo.Ingestion;

[ExcludeFromCodeCoverage(Justification = "Host bootstrap, exercised by running the function app.")]
internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.ConfigureFunctionsWebApplication();
        builder.Services.AddIngestion(builder.Configuration);
        builder.Build().Run();
    }
}
