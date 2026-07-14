using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Abstractions.Time;
using TaskBridge.Application.Common;
using TaskBridge.Domain.Tasks;

namespace TaskBridge.Application.Tasks.ChangeTaskStatus;

/// <summary>
/// Represents ChangeTaskStatusHandler.
/// </summary>
public sealed class ChangeTaskStatusHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionService _permissionService;
    private readonly IClock _clock;

    public ChangeTaskStatusHandler(
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

    public async Task<Result<ChangeTaskStatusResult>> Handle(
        ChangeTaskStatusCommand command,
        CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(x => x.Id == command.TaskId, cancellationToken);

        if (task is null)
        {
            return Result<ChangeTaskStatusResult>.Failure(
                Error.NotFound("Task.NotFound", "Task was not found"));
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(x => x.Id == task.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result<ChangeTaskStatusResult>.Failure(
                Error.NotFound("Project.NotFound", "Project was not found"));
        }

        if (project.Status == Domain.Projects.ProjectStatus.Archived)
        {
            return Result<ChangeTaskStatusResult>.Failure(
                Error.Conflict("Project.Archived", "Cannot change task status in archived project"));
        }

        if (task.Version != command.ExpectedVersion)
        {
            return Result<ChangeTaskStatusResult>.Failure(
                Error.Conflict("Task.VersionConfilct", "Task was changed by another user"));
        }

        var canChangeStatus = await _permissionService.CanChangeTaskStatusAsync(
            _currentUser.UserId,
            command.TaskId,
            cancellationToken);
        
        if (!canChangeStatus)
        {
            return Result<ChangeTaskStatusResult>.Failure(
                Error.Forbidden("Task.ChangeStatusForbidden", "User cannot change this task status"));
        }

        var oldStatus = task.Status;
        var now = _clock.UtcNow;

        task.ChangeStatus(command.NewStatus, now);

        if (oldStatus != command.NewStatus)
        {
            var history = new TaskHistory(
                task.Id,
                _currentUser.UserId,
                TaskHistoryChangeType.StatusChanged,
                oldStatus.ToString(),
                command.NewStatus.ToString(),
                now);

            _context.TaskHistory.Add(history);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = new ChangeTaskStatusResult(
            task.Id,
            task.Status,
            task.CompletedAt,
            task.UpdatedAt,
            task.Version);

        return Result<ChangeTaskStatusResult>.Success(result);
    }
}
