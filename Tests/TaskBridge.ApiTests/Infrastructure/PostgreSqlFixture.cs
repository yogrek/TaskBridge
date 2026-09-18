using Microsoft.EntityFrameworkCore;

using Testcontainers.PostgreSql;

using TaskBridge.DB;

namespace TaskBridge.ApiTests.Infrastructure;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16")
        .WithDatabase("taskbridge_api_tests")
        .WithUsername("taskbridge")
        .WithPassword("taskbridge")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateContext();
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE task_history, task_comments, tasks, projects, workspace_members, workspaces, users CASCADE;");
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new AppDbContext(options);
    }
}
