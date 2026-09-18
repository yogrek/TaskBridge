using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Comments.AddTaskComment;
using TaskBridge.Application.Projects.CreateProject;
using TaskBridge.Application.Security;
using TaskBridge.Application.Tasks.ChangeTaskStatus;
using TaskBridge.Application.Tasks.CreateTask;
using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;
using TaskBridge.IntegrationTests.Fixtures;
using TaskBridge.IntegrationTests.Helpers;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.IntegrationTests;

public sealed class HandlerPersistenceTests : PostgreSqlTestBase
{
    public HandlerPersistenceTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateProject_DuplicateName_ShouldReturnConflict()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);
        var scenario = await SeedOwnerProjectAsync(context);
        var handler = new CreateProjectHandler(
            context,
            new FakeCurrentUser { UserId = scenario.Owner.Id },
            new PermissionService(context),
            new FakeClock { UtcNow = scenario.Now });

        // Act
        var result = await handler.Handle(
            new CreateProjectCommand(scenario.Workspace.Id, scenario.Project.Name, null),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Project.NameAlreadyExists", result.Error!.Code);
    }

    [Fact]
    public async Task CreateTask_ValidOwner_ShouldPersistTaskAndHistory()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);
        var scenario = await SeedOwnerProjectAsync(context);
        var handler = new CreateTaskHandler(
            context,
            new FakeCurrentUser { UserId = scenario.Owner.Id },
            new PermissionService(context),
            new FakeClock { UtcNow = scenario.Now.AddMinutes(1) });

        // Act
        var result = await handler.Handle(
            new CreateTaskCommand(scenario.Project.Id, "Created task", null, null, TaskPriority.High, null),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, await context.Tasks.CountAsync(x => x.Id == result.Value!.TaskId));
        Assert.Contains(await context.TaskHistory.ToListAsync(), x =>
            x.TaskId == result.Value!.TaskId && x.ChangeType == TaskHistoryChangeType.TaskCreated);
    }

    [Fact]
    public async Task ChangeTaskStatus_StaleVersion_ShouldReturnConflict()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);
        var scenario = await SeedOwnerProjectAsync(context, includeTask: true);
        var handler = new ChangeTaskStatusHandler(
            context,
            new FakeCurrentUser { UserId = scenario.Owner.Id },
            new PermissionService(context),
            new FakeClock { UtcNow = scenario.Now.AddMinutes(1) });

        // Act
        var result = await handler.Handle(
            new ChangeTaskStatusCommand(scenario.Task!.Id, TaskStatus.Done, scenario.Task.Version + 1),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Task.VersionConflict", result.Error!.Code);
    }

    [Fact]
    public async Task AddComment_Member_ShouldPersistCommentAndHistory()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);
        var scenario = await SeedOwnerProjectAsync(context, includeTask: true);
        var handler = new AddTaskCommentHandler(
            context,
            new FakeCurrentUser { UserId = scenario.Owner.Id },
            new PermissionService(context),
            new FakeClock { UtcNow = scenario.Now.AddMinutes(1) });

        // Act
        var result = await handler.Handle(
            new AddTaskCommentCommand(scenario.Task!.Id, "Comment"),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, await context.TaskComments.CountAsync(x => x.Id == result.Value!.CommentId));
        Assert.Contains(await context.TaskHistory.ToListAsync(), x =>
            x.TaskId == scenario.Task.Id && x.ChangeType == TaskHistoryChangeType.CommentAdded);
    }

    private static async Task<OwnerProjectScenario> SeedOwnerProjectAsync(
        TaskBridge.DB.AppDbContext context,
        bool includeTask = false)
    {
        var now = new DateTimeOffset(2026, 9, 18, 12, 0, 0, TimeSpan.Zero);
        var owner = new User("owner@test.com", "Owner", "hash", now);
        var workspace = new Workspace("Workspace", owner.Id, now);
        var project = new Project(workspace.Id, "Project", null, now);
        TaskItem? task = includeTask
            ? new TaskItem(project.Id, "Task", null, owner.Id, null, TaskPriority.Normal, null, now)
            : null;

        context.AddRange(
            owner,
            workspace,
            new WorkspaceMember(workspace.Id, owner.Id, WorkspaceRole.Owner, now),
            project);
        if (task is not null)
            context.Tasks.Add(task);
        await context.SaveChangesAsync();

        return new OwnerProjectScenario(now, owner, workspace, project, task);
    }

    private sealed record OwnerProjectScenario(
        DateTimeOffset Now,
        User Owner,
        Workspace Workspace,
        Project Project,
        TaskItem? Task);
}
