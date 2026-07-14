using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Common;

namespace TaskBridge.Application.Tasks.GetTaskDetails;

/// <summary>
/// Represents GetTaskDetailsHandler.
/// </summary>
public sealed class GetTaskDetailsHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetTaskDetailsHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetTaskDetailsResult>> Handle(
        GetTaskDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.TaskId, cancellationToken);

        if (task is null)
        {
            return Result<GetTaskDetailsResult>.Failure(
                Error.NotFound("Task.NotFound", "Task was not found."));
        }

        var project = await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == task.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result<GetTaskDetailsResult>.Failure(
                Error.NotFound("Project.NotFound", "Project was not found"));
        }

        var isMember = await _context.WorkspaceMembers
            .AnyAsync(
                x => x.WorkspaceId == project.WorkspaceId &&
                     x.UserId == _currentUser.UserId,
                cancellationToken);

        if (!isMember)
        {
            return Result<GetTaskDetailsResult>.Failure(
                Error.Forbidden("Task.AccessForbidden", "User has not access to this task"));
        }

        var comments = await _context.TaskComments
            .AsNoTracking()
            .Where(x => x.TaskId == task.Id)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new TaskCommentItem(
                x.Id,
                x.AuthorId,
                x.Text,
                x.CreatedAt))
            .ToListAsync(cancellationToken);

        var history = await _context.TaskHistory
            .AsNoTracking()
            .Where(x => x.TaskId == task.Id)
            .OrderByDescending(x => x.ChangedAt)
            .Select(x => new TaskHistoryItem(
                x.Id,
                x.ChangedBy,
                x.ChangeType,
                x.OldValue,
                x.NewValue,
                x.ChangedAt))
            .ToListAsync(cancellationToken);

        var result = new GetTaskDetailsResult(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.AuthorId,
            task.AssigneeId,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt,
            task.CompletedAt,
            task.Version,
            comments,
            history);

        return Result<GetTaskDetailsResult>.Success(result);
    }
}
