using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;
using TaskBridge.Application.Security;
using TaskBridge.IntegrationTests.Fixtures;
using TaskBridge.IntegrationTests.Helpers;

namespace TaskBridge.IntegrationTests;

public sealed class PermissionServiceTests : PostgreSqlTestBase
{
    public PermissionServiceTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_From_Another_Workspace_Should_Not_Change_Task_Status()
    {
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);

        var now = DateTimeOffset.UtcNow;
        var userA = new User("user-a@test.com", "hash", "User A", now);
        var userB = new User("user-b@test.com", "hash", "User B", now);
        var workspaceA = new Workspace("Workspace A", userA.Id, now);
        var workspaceB = new Workspace("Workspace B", userB.Id, now);
        var memberA = new WorkspaceMember(workspaceA.Id, userA.Id, WorkspaceRole.Owner, now);
        var memberB = new WorkspaceMember(workspaceB.Id, userB.Id, WorkspaceRole.Owner, now);
        var projectB = new Project(workspaceB.Id, "Project B", null, now);
        var taskB = new TaskItem(
            projectB.Id,
            "Task B",
            null,
            userB.Id,
            null,
            TaskPriority.Normal,
            null,
            now);

        context.AddRange(
            userA,
            userB,
            workspaceA,
            workspaceB,
            memberA,
            memberB,
            projectB,
            taskB);
        await context.SaveChangesAsync();

        var permissionService = new PermissionService(context);

        var canChangeStatus = await permissionService.CanChangeTaskStatusAsync(
            userA.Id,
            taskB.Id,
            CancellationToken.None);

        Assert.False(canChangeStatus);
    }
}
