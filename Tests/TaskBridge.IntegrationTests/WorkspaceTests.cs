using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Workspaces.CreateWorkspace;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;
using TaskBridge.IntegrationTests.Fixtures;
using TaskBridge.IntegrationTests.Helpers;

namespace TaskBridge.IntegrationTests;

public sealed class WorkspaceTests : PostgreSqlTestBase
{
    public WorkspaceTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateWorkspace_Should_Create_OwnerMembership()
    {
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);

        var now = new DateTimeOffset(2026, 9, 17, 10, 0, 0, TimeSpan.Zero);

        var user = new User(
            "user@test.com",
            "hash1",
            "User 1",
            now);

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var currentUser = new FakeCurrentUser { UserId = user.Id };
        var clock = new FakeClock { UtcNow = now };

        var handler = new CreateWorkspaceHandler(context, currentUser, clock);

        var result = await handler.Handle(new CreateWorkspaceCommand("Test Workspace"), CancellationToken.None);

        Assert.True(result.IsSuccess);

        var workspace = await context.Workspaces.SingleAsync();
        var member = await context.WorkspaceMembers.SingleAsync();

        Assert.Equal(workspace.Id, member.WorkspaceId);
        Assert.Equal(user.Id, member.UserId);
        Assert.Equal(WorkspaceRole.Owner, member.Role);
    }
}
