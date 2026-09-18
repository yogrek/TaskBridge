using TaskBridge.Application.Security;
using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;
using TaskBridge.IntegrationTests.Fixtures;
using TaskBridge.IntegrationTests.Helpers;

namespace TaskBridge.IntegrationTests;

public sealed class PermissionMatrixTests : PostgreSqlTestBase
{
    public PermissionMatrixTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData(WorkspaceRole.Owner, true, true, true, true, true)]
    [InlineData(WorkspaceRole.Admin, true, true, true, true, true)]
    [InlineData(WorkspaceRole.ProjectManager, true, true, true, true, true)]
    [InlineData(WorkspaceRole.Member, false, true, false, true, true)]
    [InlineData(WorkspaceRole.Viewer, false, false, false, false, false)]
    public async Task Permissions_ForWorkspaceRole_ShouldMatchMatrix(
        WorkspaceRole role,
        bool canCreateProject,
        bool canCreateTask,
        bool canAssignTask,
        bool canChangeOwnTaskStatus,
        bool canAddComment)
    {
        // Arrange
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);
        var scenario = await SeedWorkspaceAsync(context, role);
        var permissions = new PermissionService(context);

        // Act
        var actualCreateProject = await permissions.CanCreateProjectAsync(
            scenario.Actor.Id, scenario.Workspace.Id, CancellationToken.None);
        var actualCreateTask = await permissions.CanCreateTaskAsync(
            scenario.Actor.Id, scenario.Project.Id, CancellationToken.None);
        var actualAssignTask = await permissions.CanAssignTaskAsync(
            scenario.Actor.Id, scenario.Project.Id, scenario.Assignee.Id, CancellationToken.None);
        var actualChangeStatus = await permissions.CanChangeTaskStatusAsync(
            scenario.Actor.Id, scenario.Task.Id, CancellationToken.None);
        var actualAddComment = await permissions.CanAddCommentAsync(
            scenario.Actor.Id, scenario.Task.Id, CancellationToken.None);

        // Assert
        Assert.Equal(canCreateProject, actualCreateProject);
        Assert.Equal(canCreateTask, actualCreateTask);
        Assert.Equal(canAssignTask, actualAssignTask);
        Assert.Equal(canChangeOwnTaskStatus, actualChangeStatus);
        Assert.Equal(canAddComment, actualAddComment);
    }

    [Fact]
    public async Task NonMember_AnyWorkspaceAction_ShouldBeDenied()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create(Fixture.ConnectionString);
        var scenario = await SeedWorkspaceAsync(context, WorkspaceRole.Owner);
        var nonMember = new User("outsider@test.com", "Outsider", "hash", scenario.Now);
        context.Users.Add(nonMember);
        await context.SaveChangesAsync();
        var permissions = new PermissionService(context);

        // Act
        var results = new[]
        {
            await permissions.CanCreateProjectAsync(nonMember.Id, scenario.Workspace.Id, CancellationToken.None),
            await permissions.CanCreateTaskAsync(nonMember.Id, scenario.Project.Id, CancellationToken.None),
            await permissions.CanAssignTaskAsync(nonMember.Id, scenario.Project.Id, scenario.Assignee.Id, CancellationToken.None),
            await permissions.CanChangeTaskStatusAsync(nonMember.Id, scenario.Task.Id, CancellationToken.None),
            await permissions.CanAddCommentAsync(nonMember.Id, scenario.Task.Id, CancellationToken.None)
        };

        // Assert
        Assert.All(results, Assert.False);
    }

    private static async Task<WorkspaceScenario> SeedWorkspaceAsync(
        TaskBridge.DB.AppDbContext context,
        WorkspaceRole actorRole)
    {
        var now = new DateTimeOffset(2026, 9, 18, 12, 0, 0, TimeSpan.Zero);
        var owner = new User("owner@test.com", "Owner", "hash", now);
        var actor = new User("actor@test.com", "Actor", "hash", now);
        var assignee = new User("assignee@test.com", "Assignee", "hash", now);
        var workspace = new Workspace("Workspace", owner.Id, now);
        var project = new Project(workspace.Id, "Project", null, now);
        var task = new TaskItem(project.Id, "Task", null, actor.Id, null, TaskPriority.Normal, null, now);

        context.AddRange(
            owner,
            actor,
            assignee,
            workspace,
            new WorkspaceMember(workspace.Id, owner.Id, WorkspaceRole.Owner, now),
            new WorkspaceMember(workspace.Id, actor.Id, actorRole, now),
            new WorkspaceMember(workspace.Id, assignee.Id, WorkspaceRole.Member, now),
            project,
            task);
        await context.SaveChangesAsync();

        return new WorkspaceScenario(now, actor, assignee, workspace, project, task);
    }

    private sealed record WorkspaceScenario(
        DateTimeOffset Now,
        User Actor,
        User Assignee,
        Workspace Workspace,
        Project Project,
        TaskItem Task);
}
