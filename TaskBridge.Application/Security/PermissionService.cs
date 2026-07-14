using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Domain.Workspaces;

namespace TaskBridge.Application.Security;

public sealed class PermissionService : IPermissionService
{
    private readonly IAppDbContext _context;

    public PermissionService(IAppDbContext context) => _context = context;

    public async Task<bool> CanCreateProjectAsync(Guid userId, Guid workspaceId, CancellationToken cancellationToken)
    {
        var role = await GetRoleAsync(userId, workspaceId, cancellationToken);

        return role is WorkspaceRole.Owner or WorkspaceRole.Admin or WorkspaceRole.ProjectManager;
    }

    public async Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId, CancellationToken cancellationToken)
    {
        var workspaceId = await _context.Projects
            .Where(x => x.Id == projectId)
            .Select(x => x.WorkspaceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (workspaceId == Guid.Empty)
            return false;

        var role = await GetRoleAsync(userId, workspaceId, cancellationToken);

        return role is WorkspaceRole.Admin or WorkspaceRole.ProjectManager or WorkspaceRole.Member;
    }

    public async Task<bool> CanAssignTaskAsync(Guid userId, Guid projectId, Guid assigneeId, CancellationToken cancellationToken)
    {
        var workspaceId = await _context.Projects
            .Where(x => x.Id == projectId)
            .Select(x => x.WorkspaceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (workspaceId == Guid.Empty)
            return false;

        var assigneeIsMember = await _context.WorkspaceMembers
            .AnyAsync(x => x.WorkspaceId == workspaceId &&
                           x.UserId == assigneeId,
                           cancellationToken);

        if (!assigneeIsMember)
            return false;

        var role = await GetRoleAsync(userId, workspaceId, cancellationToken);

        return role is WorkspaceRole.Owner or WorkspaceRole.Admin or WorkspaceRole.ProjectManager;
    }

    public async Task<bool> CanChangeTaskStatusAsync(Guid userId, Guid taskId, CancellationToken cancellationToken)
    {
        var data = await (
            from task in _context.Tasks
            join project in _context.Projects on task.ProjectId equals project.Id
            where task.Id == taskId
            select new
            {
                project.WorkspaceId,
                task.AuthorId,
                task.AssigneeId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
            return false;

        var role = await GetRoleAsync(userId, data.WorkspaceId, cancellationToken);

        if (role is WorkspaceRole.Owner or
            WorkspaceRole.Admin or
            WorkspaceRole.ProjectManager)
        {
            return true;
        }

        if (role == WorkspaceRole.Member)
            return data.AuthorId == userId || data.AssigneeId == userId;

        return false;
    }

    public async Task<bool> CanAddCommentAsync(Guid userId, Guid taskId, CancellationToken cancellationToken)
    {
        var workspaceId = await (
            from task in _context.Tasks
            join project in _context.Projects on task.ProjectId equals project.Id
            where task.Id == taskId
            select project.WorkspaceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (workspaceId == Guid.Empty)
            return false;

        var role = await GetRoleAsync(userId, workspaceId, cancellationToken);

        return role is WorkspaceRole.Owner or WorkspaceRole.Admin or WorkspaceRole.ProjectManager or WorkspaceRole.Member;
    }

    private async Task<WorkspaceRole?> GetRoleAsync(Guid userId, Guid wodkspaceId, CancellationToken cancellationToken)
        => await _context.WorkspaceMembers
        .Where(x => x.WorkspaceId == wodkspaceId && x.UserId == userId)
        .Select(x => (WorkspaceRole?)x.Role)
        .FirstOrDefaultAsync(cancellationToken);
}
