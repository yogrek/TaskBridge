namespace TaskBridge.ApiTests.Infrastructure;

[Collection(ApiTestCollection.Name)]
public abstract class ApiTestBase : IAsyncLifetime
{
    protected ApiTestBase(PostgreSqlFixture fixture)
    {
        Fixture = fixture;
        Factory = new TaskBridgeApiFactory(fixture.ConnectionString);
    }

    protected PostgreSqlFixture Fixture { get; }
    protected TaskBridgeApiFactory Factory { get; }

    public Task InitializeAsync() => Fixture.ResetDatabaseAsync();

    public Task DisposeAsync()
    {
        Factory.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ApiTestCollection : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "TaskBridge API tests";
}
