using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Security;
using TaskBridge.Application.Tasks.ChangeTaskStatus;
using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;
using TaskBridge.IntegrationTests.Fixtures;
using TaskBridge.IntegrationTests.Helpers;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.IntegrationTests;

public sealed class ChangeTaskStatusTests : PostgreSqlTestBase
{
    public ChangeTaskStatusTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task ChangeTaskStatus_Should_Persist_Task_And_History()
    {
        var createdAt = new DateTimeOffset(2026, 9, 18, 10, 0, 0, TimeSpan.Zero);
        var completedAt = createdAt.AddHours(1);

        Guid taskId;
        Guid userId;

        await using (var setupContext = TestDbContextFactory.Create(Fixture.ConnectionString))
        {
            var user = new User("user@test.com", "hash", "User", createdAt);
            var workspace = new Workspace("Workspace", user.Id, createdAt);
            var member = new WorkspaceMember(workspace.Id, user.Id, WorkspaceRole.Owner, createdAt);
            var project = new Project(workspace.Id, "Project", null, createdAt);
            var task = new TaskItem(
                project.Id,
                "Task",
                null,
                user.Id,
                null,
                TaskPriority.Normal,
                null,
                createdAt);

            setupContext.AddRange(user, workspace, member, project, task);
            await setupContext.SaveChangesAsync();

            taskId = task.Id;
            userId = user.Id;

            var handler = new ChangeTaskStatusHandler(
                setupContext,
                new FakeCurrentUser { UserId = userId },
                new PermissionService(setupContext),
                new FakeClock { UtcNow = completedAt });

            var result = await handler.Handle(
                new ChangeTaskStatusCommand(taskId, TaskStatus.Done, task.Version),
                CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        await using var verificationContext = TestDbContextFactory.Create(Fixture.ConnectionString);

        var taskInDatabase = await verificationContext.Tasks
            .AsNoTracking()
            .SingleAsync(x => x.Id == taskId);
        var historyInDatabase = await verificationContext.TaskHistory
            .AsNoTracking()
            .SingleAsync(x => x.TaskId == taskId);

        Assert.Equal(TaskStatus.Done, taskInDatabase.Status);
        Assert.NotNull(taskInDatabase.CompletedAt);
        Assert.Equal(TaskHistoryChangeType.StatusChanged, historyInDatabase.ChangeType);
    }
}
