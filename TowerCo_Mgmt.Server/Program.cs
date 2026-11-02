using System.Text.Json.Serialization;
using Scalar.AspNetCore;
using TowerCo.Application;
using TowerCo.Application.Constants;
using TowerCo.Application.Dashboard;
using TowerCo.Application.Health;
using TowerCo.Domain.Monitoring;
using TowerCo.Infrastructure;
using TowerCo_Mgmt.Server.ExceptionHandling;

namespace TowerCo_Mgmt.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);

        var app = builder.Build();
        await app.Services.InitialiseDatabaseAsync();
        ConfigurePipeline(app);

        await app.RunAsync();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddOptions<DashboardOptions>().BindConfiguration(ConfigurationSections.Dashboard)
            .ValidateDataAnnotations().ValidateOnStart();
        builder.Services.AddOptions<HealthOptions>().BindConfiguration(ConfigurationSections.Health)
            .ValidateDataAnnotations().ValidateOnStart();
        builder.Services.AddOptions<SensorThresholdOptions>().BindConfiguration(ConfigurationSections.AlarmThresholds)
            .Validate(t => Enum.GetValues<SensorType>().All(t.ContainsKey), ServerConstants.MissingSensorThreshold).ValidateOnStart();
        // AUTH (out of scope for this demo): register authentication (e.g. Microsoft Entra ID JWT bearer via
        // AddAuthentication().AddMicrosoftIdentityWebApi) and role policies Operations, Commercial, Manager here.
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<DomainExceptionHandler>();
        builder.Services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddOpenApi();
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseDefaultFiles();
        app.MapStaticAssets();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options => options.WithTitle(ServerConstants.ApiReferenceTitle));
        }
        app.UseHttpsRedirection();
        // AUTH (out of scope for this demo): app.UseAuthentication() and app.UseAuthorization() belong here, before MapControllers.
        app.MapControllers();
        app.MapFallbackToFile("/index.html");
    }
}
