using System.Net;

using Npgsql;

using TaskBridge.ApiTests.Infrastructure;

namespace TaskBridge.ApiTests;

public sealed class HealthApiTests : ApiTestBase
{
    public HealthApiTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Live_WhenApiIsRunning_ShouldReturnOk()
    {
        using var client = Factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_WhenPostgreSqlIsAvailable_ShouldReturnOk()
    {
        using var client = Factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_WhenPostgreSqlIsUnavailable_ShouldReturnServiceUnavailable()
    {
        var connectionString = new NpgsqlConnectionStringBuilder(Fixture.ConnectionString)
        {
            Host = "127.0.0.1",
            Port = 1,
            Timeout = 1,
            Pooling = false
        }.ConnectionString;

        using var factory = new TaskBridgeApiFactory(connectionString);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
