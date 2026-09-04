using Microsoft.EntityFrameworkCore;

using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;

namespace TaskBridge.DB.Seed;

/// <summary>
/// Seeds development-only data into an empty database.
/// </summary>
public static class DevelopmentDataSeeder
{
    /// <summary>
    /// Adds a demonstration workspace with a project and a task when it is absent.
    /// </summary>
    public static async Task SeedAsync(
        AppDbContext context,
        CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        var now = DateTimeOffset.UtcNow;

        var user = new User(
            email: "admin@taskbridge.local",
            displayName: "TaskBridge Admin",
            passwordHash: "dev-password-hash",
            createdAt: now);

        var workspace = new Workspace(
            name: "Demo Wordkspace",
            ownerId: user.Id,
            createdAt: now);

        var member = new WorkspaceMember(
            workspace.Id,
            user.Id,
            WorkspaceRole.Owner,
            now);

        var project = new Project(
            workspace.Id,
            "Demo project",
            "Project for local development.",
            now);

        var task = new TaskItem(
            project.Id,
            "Prepare MVP architecture.",
            "Create first architecture documents for TaskBridge.",
            user.Id,
            user.Id,
            TaskPriority.High,
            now.AddDays(7),
            now);

        var history = new TaskHistory(
            task.Id,
            user.Id,
            TaskHistoryChangeType.TaskCreated,
            oldValue: null,
            newValue: task.Title,
            changedAt: now);

        context.Users.Add(user);
        context.Workspaces.Add(workspace);
        context.WorkspaceMembers.Add(member);
        context.Projects.Add(project);
        context.Tasks.Add(task);
        context.TaskHistory.Add(history);

        await context.SaveChangesAsync(cancellationToken);
    }
}
