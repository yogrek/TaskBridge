using TaskBridge.Domain.Tasks;

namespace TaskBridge.Application.Tasks.CreateTask;

/// <summary>
/// Represents CreateTaskCommand.
/// </summary>
public sealed record CreateTaskCommand(Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssigneeId,
    TaskPriority Priority,
    DateTimeOffset? DueDate);
