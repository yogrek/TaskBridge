using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Abstractions.Time;
using TaskBridge.Application.Common;
using TaskBridge.Domain.Tasks;

namespace TaskBridge.Application.Tasks.CreateTask;

/// <summary>
/// Represents CreateTaskHandler.
/// </summary>
public sealed class CreateTaskHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionService _permissionService;
    private readonly IClock _clock;

    public CreateTaskHandler(
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

    public async Task<Result<CreateTaskResult>> Handle(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return Result<CreateTaskResult>.Failure(
                Error.Validation("Task.TitleRequired", "Task title is required"));
        }

        var project = await _context.Projects
            .FirstOrDefaultAsync(x => x.Id == command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result<CreateTaskResult>.Failure(
                Error.NotFound("Project.NotFound", "Project was not found."));
        }

        if (project.Status == Domain.Projects.ProjectStatus.Archived)
        {
            return Result<CreateTaskResult>.Failure(
                Error.Conflict("Project.Archived", "Cannot create tasks in archived project."));
        }

        var canCreateTask = await _permissionService.CanCreateTaskAsync(
            _currentUser.UserId,
            command.ProjectId,
            cancellationToken);

        if (!canCreateTask)
        {
            return Result<CreateTaskResult>.Failure(
                Error.Forbidden("Task.CreateForbidden", "User cannot create tasks in this project."));
        }

        if (command.AssigneeId is not null)
        {
            var canAssign = await _permissionService.CanAssignTaskAsync(
                _currentUser.UserId,
                command.ProjectId,
                command.AssigneeId.Value,
                cancellationToken);

            if (!canAssign)
            {
                return Result<CreateTaskResult>.Failure(
                    Error.Forbidden("Task.AssignForbidden", "User cannot assign this task to the specified user."));
            }
        }

        var now = _clock.UtcNow;

        var task = new TaskItem(
            command.ProjectId,
            command.Title,
            command.Description,
            _currentUser.UserId,
            command.AssigneeId,
            command.Priority,
            command.DueDate,
            now);

        var history = new TaskHistory(
            task.Id,
            _currentUser.UserId,
            TaskHistoryChangeType.TaskCreated,
            oldValue: null,
            newValue: task.Title,
            changedAt: now);

        _context.Tasks.Add(task);
        _context.TaskHistory.Add(history);

        await _context.SaveChangesAsync();

        var result = new CreateTaskResult(
            task.Id,
            task.ProjectId,
            task.Title,
            task.Status,
            task.Priority,
            task.AuthorId,
            task.AssigneeId,
            task.DueDate,
            task.CreatedAt);

        return Result<CreateTaskResult>.Success(result);
    }
}
