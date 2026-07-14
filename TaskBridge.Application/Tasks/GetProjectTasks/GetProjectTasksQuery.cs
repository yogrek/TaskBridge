using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Application.Tasks.GetProjectTasks;

/// <summary>
/// Represents GetProjectTasksQuery.
/// </summary>
public sealed record GetProjectTasksQuery(
    Guid ProjectId,
    TaskStatus? Status,
    Guid? AssigneeId,
    int Page,
    int PageSize);
