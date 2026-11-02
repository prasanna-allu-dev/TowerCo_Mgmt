using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TowerCo.Ingestion.Processing;

namespace TowerCo.Ingestion;

/// <summary>Registers the sensor ingestion pipeline.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddIngestion(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AlarmThresholdOptions>(configuration.GetSection(IngestionConstants.AlarmThresholdsSection));
        services.AddSingleton<ISensorReadingParser, SensorReadingParser>();
        services.AddSingleton<IAlarmPublisher, LoggingAlarmPublisher>();
        services.AddSingleton<ISensorReadingProcessor, SensorReadingProcessor>();
        return services;
    }
}
