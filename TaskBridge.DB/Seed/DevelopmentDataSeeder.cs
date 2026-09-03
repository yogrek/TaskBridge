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
    private const string DeveloperEmail = "developer@taskbridge.local";

    /// <summary>
    /// Adds a demonstration workspace with a project and a task when it is absent.
    /// </summary>
    public static async Task SeedAsync(
        AppDbContext context,
        CancellationToken cancellationToken = default)
    {
        var dataAlreadySeeded = await context.Users
            .AnyAsync(user => user.Email == DeveloperEmail, cancellationToken);

        if (dataAlreadySeeded)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User(
            DeveloperEmail,
            "TaskBridge Developer",
            "Development-only-password-hash",
            now);

        var workspace = new Workspace("TaskBridge", user.Id, now);
        var member = new WorkspaceMember(workspace.Id, user.Id, WorkspaceRole.Owner, now);
        var project = new Project(
            workspace.Id,
            "Getting started",
            "Initial project created for local development.",
            now);
        var task = new TaskItem(
            project.Id,
            "Configure TaskBridge",
            "Configure the local application and create the first project task.",
            user.Id,
            user.Id,
            TaskPriority.Normal,
            now.AddDays(7),
            now);
        var comment = new TaskComment(
            task.Id,
            user.Id,
            "This is development seed data.",
            now,
            now);
        var taskCreated = new TaskHistory(
            task.Id,
            user.Id,
            TaskHistoryChangeType.TaskCreated,
            oldValue: null,
            newValue: task.Title,
            changedAt: now);
        var commentAdded = new TaskHistory(
            task.Id,
            user.Id,
            TaskHistoryChangeType.CommentAdded,
            oldValue: null,
            newValue: comment.Text,
            changedAt: now);

        context.AddRange(user, workspace, member, project, task, comment, taskCreated, commentAdded);

        await context.SaveChangesAsync(cancellationToken);
    }
}
