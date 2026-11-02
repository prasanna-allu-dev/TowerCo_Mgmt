using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TowerCo.Application;
using TowerCo.Infrastructure;

namespace TowerCo.Tests.Application;

/// <summary>Real Application and Infrastructure wiring over a private, seeded in-memory SQLite database.</summary>
public sealed class SeededServices : IAsyncDisposable
{
    private readonly SqliteConnection _keepAlive;
    private readonly ServiceProvider _provider;

    private SeededServices(string connectionString)
    {
        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{InfrastructureConstants.ConnectionStringName}"] = connectionString
            })
            .Build();
        _provider = new ServiceCollection().AddLogging().AddApplication()
            .AddInfrastructure(configuration).BuildServiceProvider();
    }

    public static async Task<SeededServices> CreateAsync()
    {
        var services = new SeededServices($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared");
        await services._provider.InitialiseDatabaseAsync();
        return services;
    }

    /// <summary>Resolves a service in a fresh scope, as one HTTP request would.</summary>
    public T Get<T>() where T : notnull => _provider.CreateScope().ServiceProvider.GetRequiredService<T>();

    public async ValueTask DisposeAsync()
    {
        await _provider.DisposeAsync();
        await _keepAlive.DisposeAsync();
    }
}
