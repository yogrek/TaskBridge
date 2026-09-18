namespace TaskBridge.IntegrationTests.Fixtures;

[Collection(PostgreSqlCollection.Name)]
public abstract class PostgreSqlTestBase : IAsyncLifetime
{
    protected PostgreSqlTestBase(PostgreSqlFixture fixture) => Fixture = fixture;

    protected PostgreSqlFixture Fixture { get; }

    public Task InitializeAsync() => Fixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
