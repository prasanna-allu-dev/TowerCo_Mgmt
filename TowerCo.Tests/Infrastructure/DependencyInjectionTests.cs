using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TowerCo.Application;
using TowerCo.Infrastructure;
using TowerCo.Infrastructure.Exceptions;
using TowerCo.Infrastructure.Persistence;

namespace TowerCo.Tests.Infrastructure;

public class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_MissingConnectionString_ThrowsMissingConfigurationException()
    {
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<MissingConfigurationException>(
            () => new ServiceCollection().AddInfrastructure(configuration));

        Assert.Contains(InfrastructureConstants.ConnectionStringName, exception.Message);
    }

    [Fact]
    public async Task InitialiseDatabaseAsync_ValidConfiguration_CreatesConnectableDatabase()
    {
        var provider = BuildProvider($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared");

        await provider.InitialiseDatabaseAsync();

        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TowerCoDbContext>();
        Assert.True(await context.Database.CanConnectAsync());
    }

    private static ServiceProvider BuildProvider(string connectionString)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{InfrastructureConstants.ConnectionStringName}"] = connectionString
            })
            .Build();
        return new ServiceCollection().AddApplication().AddInfrastructure(configuration).BuildServiceProvider();
    }
}
