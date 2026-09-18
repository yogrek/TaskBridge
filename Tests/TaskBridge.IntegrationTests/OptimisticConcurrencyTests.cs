using TaskBridge.IntegrationTests.Fixtures;

using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;
using TaskBridge.IntegrationTests.Helpers;

using Microsoft.EntityFrameworkCore;

namespace TaskBridge.IntegrationTests;

public sealed class OptimisticConcurrencyTests : PostgreSqlTestBase
{
    public OptimisticConcurrencyTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Concurrent_Update_Should_Throw()
    {
        var task = await SeedTask();

        await using var contextA = TestDbContextFactory.Create(Fixture.ConnectionString);
        await using var contextB = TestDbContextFactory.Create(Fixture.ConnectionString);

        var taskA = await contextA.Tasks.SingleAsync(x => x.Id == task.Id);
        var taskB = await contextB.Tasks.SingleAsync(x => x.Id == task.Id);

        var now = DateTimeOffset.UtcNow;

        taskB.ChangePriority(TaskPriority.High, now);
        await contextB.SaveChangesAsync();

        taskA.ChangePriority(TaskPriority.Critical, now.AddMinutes(1));

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => contextA.SaveChangesAsync());
    }

    private async Task<TaskItem> SeedTask()
    {
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);

        var now = DateTimeOffset.UtcNow;
        var user = new User("user@test.com", "hash", "User", now);
        var workspace = new Workspace("Workspace", user.Id, now);
        var member = new WorkspaceMember(workspace.Id, user.Id, WorkspaceRole.Owner, now);
        var project = new Project(workspace.Id, "Project", null, now);
        var task = new TaskItem(
            project.Id,
            "Task",
            null,
            user.Id,
            null,
            TaskPriority.Normal,
            null,
            now);

        context.AddRange(user, workspace, member, project, task);
        await context.SaveChangesAsync();

        return task;
    }
}
