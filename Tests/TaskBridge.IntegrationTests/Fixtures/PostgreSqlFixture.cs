using Testcontainers.PostgreSql;

using Microsoft.EntityFrameworkCore;

using TaskBridge.IntegrationTests.Helpers;

namespace TaskBridge.IntegrationTests.Fixtures;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("taskbridge_tests")
        .WithUsername("taskbridge")
        .WithPassword("taskbridge")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = TestDbContextFactory.Create(ConnectionString);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = TestDbContextFactory.Create(ConnectionString);

        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE task_history, task_comments, tasks, projects, workspace_members, workspaces, users CASCADE;");
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}
