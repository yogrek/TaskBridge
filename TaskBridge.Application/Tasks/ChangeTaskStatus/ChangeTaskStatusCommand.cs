using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Application.Tasks.ChangeTaskStatus;

/// <summary>
/// Represents ChangeTaskStatusCommand.
/// </summary>
public sealed record ChangeTaskStatusCommand(
    Guid TaskId,
    TaskStatus NewStatus,
    uint ExpectedVersion);
