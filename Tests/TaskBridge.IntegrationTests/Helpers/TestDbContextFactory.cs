using Microsoft.EntityFrameworkCore;

using TaskBridge.DB;

namespace TaskBridge.IntegrationTests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
