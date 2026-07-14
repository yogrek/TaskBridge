using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Abstractions.Time;
using TaskBridge.Application.Common;
using TaskBridge.Domain.Tasks;

namespace TaskBridge.Application.Comments.AddTaskComment;

/// <summary>
/// Represents AddTaskCommentHandler.
/// </summary>
public class AddTaskCommentHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionService _permissionService;
    private readonly IClock _clock;

    public AddTaskCommentHandler(
        IAppDbContext context,
        ICurrentUser currentUser,
        IPermissionService permissionService,
        IClock clock)
    {
        _context = context;
        _currentUser = currentUser;
        _permissionService = permissionService;
        _clock = clock;
    }

    public async Task<Result<AddTaskCommentResult>> Handle(
        AddTaskCommentCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Text))
        {
            return Result<AddTaskCommentResult>.Failure(
                Error.Validation("Comment.TextRequired", "Comment text is required"));
        }

        var task = await _context.Tasks
            .FirstOrDefaultAsync(x => x.Id == command.TaskId, cancellationToken);

        if (task is null)
        {
            return Result<AddTaskCommentResult>.Failure(
                Error.NotFound("Task.NotFound", "Task was not found"));
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(x => x.Id == task.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result<AddTaskCommentResult>.Failure(
                Error.NotFound("Project.NotFound", "Project was not found"));
        }

        if (project.Status == Domain.Projects.ProjectStatus.Archived)
        {
            return Result<AddTaskCommentResult>.Failure(
                Error.Conflict("Project.Archived", "Cannot add comments to task in archived project"));
        }

        var canAddComment = await _permissionService.CanAddCommentAsync(
            _currentUser.UserId,
            task.Id,
            cancellationToken);

        if (!canAddComment)
        {
            return Result<AddTaskCommentResult>.Failure(
                Error.Forbidden("Comment.AddForbidden", "User cannot add comments to this task status"));
        }

        var now = _clock.UtcNow;

        var comment = new TaskComment(
            task.Id,
            _currentUser.UserId,
            command.Text,
            now,
            now);

        var history = new TaskHistory(
            task.Id,
            _currentUser.UserId,
            TaskHistoryChangeType.CommentAdded,
            oldValue: null,
            newValue: command.Text,
            changedAt: now);

        _context.TaskComments.Add(comment);
        _context.TaskHistory.Add(history);

        await _context.SaveChangesAsync(cancellationToken);

        var result = new AddTaskCommentResult(
            comment.Id,
            comment.TaskId,
            comment.AuthorId,
            comment.Text,
            comment.CreatedAt);

        return Result<AddTaskCommentResult>.Success(result);
    }
}
