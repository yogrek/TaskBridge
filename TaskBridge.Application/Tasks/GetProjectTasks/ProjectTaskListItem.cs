using TaskBridge.Domain.Tasks;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Application.Tasks.GetProjectTasks;

public sealed record ProjectTaskListItem(
    Guid TaskId,
    string Title,
    TaskStatus Status,
    TaskPriority Priority,
    Guid? AssigneeId,
    DateTimeOffset? DueDate,
    DateTimeOffset UpdatedAt,
    uint Version);
