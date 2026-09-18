using Microsoft.EntityFrameworkCore;

using TaskBridge.DB;
using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;

namespace TaskBridge.ApiTests.Infrastructure;

public static class ApiTestData
{
    public static async Task<WorkspaceTaskData> CreateWorkspaceTaskAsync(
        string connectionString,
        bool includeSecondWorkspace = false)
    {
        await using var context = CreateContext(connectionString);
        var now = new DateTimeOffset(2026, 9, 18, 12, 0, 0, TimeSpan.Zero);
        var owner = new User("owner@test.com", "Owner", "hash", now);
        var workspace = new Workspace("Workspace", owner.Id, now);
        var project = new Project(workspace.Id, "Project", null, now);
        var task = new TaskItem(project.Id, "Task", null, owner.Id, null, TaskPriority.Normal, null, now);

        context.AddRange(
            owner,
            workspace,
            new WorkspaceMember(workspace.Id, owner.Id, WorkspaceRole.Owner, now),
            project,
            task);

        User? outsider = null;
        if (includeSecondWorkspace)
        {
            outsider = new User("outsider@test.com", "Outsider", "hash", now);
            var outsiderWorkspace = new Workspace("Outsider workspace", outsider.Id, now);
            context.AddRange(
                outsider,
                outsiderWorkspace,
                new WorkspaceMember(outsiderWorkspace.Id, outsider.Id, WorkspaceRole.Owner, now));
        }

        await context.SaveChangesAsync();

        return new WorkspaceTaskData(owner, outsider, workspace, project, task);
    }

    private static AppDbContext CreateContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    public sealed record WorkspaceTaskData(
        User Owner,
        User? Outsider,
        Workspace Workspace,
        Project Project,
        TaskItem Task);
}
