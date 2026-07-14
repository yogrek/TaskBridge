using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Application.Tasks.ChangeTaskStatus;

/// <summary>
/// Represents ChangeTaskStatusResult.
/// </summary>
public sealed record ChangeTaskStatusResult(
    Guid TaskId,
    TaskStatus Status,
    DateTimeOffset? CompletedAt,
    DateTimeOffset UpdatedAt,
    uint Version);
