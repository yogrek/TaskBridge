using Microsoft.EntityFrameworkCore;

using TaskBridge.Domain.Users;
using TaskBridge.IntegrationTests.Fixtures;
using TaskBridge.IntegrationTests.Helpers;

namespace TaskBridge.IntegrationTests;

public sealed class UserPersistenceTests : PostgreSqlTestBase
{
    public UserPersistenceTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task DuplicateNormalizedEmail_Should_Fail()
    {
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);

        var now = DateTimeOffset.UtcNow;

        var user1 = new User(
            "user@test.com",
            "hash1",
            "User 1",
            now);
        var user2 = new User(
            "USER@test.com",
            "hash2",
            "User 2",
            now);

        context.Users.Add(user1);

        await context.SaveChangesAsync();

        context.Users.Add(user2);

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}
