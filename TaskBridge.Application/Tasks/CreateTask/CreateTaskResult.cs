using TaskBridge.Domain.Tasks;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Application.Tasks.CreateTask;

/// <summary>
/// Represents CreateTaskResult.
/// </summary>
public sealed record CreateTaskResult(
    Guid TaskId,
    Guid ProjectId,
    string Title,
    TaskStatus Status,
    TaskPriority Priority,
    Guid AuthorId,
    Guid? AssigneeId,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt);
