using TaskBridge.Domain.Tasks;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Application.Tasks.GetTaskDetails;

/// <summary>
/// Represents GetTaskDetailsResult.
/// </summary>
public sealed record GetTaskDetailsResult(
    Guid TaskId,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskStatus Status,
    TaskPriority Priority,
    Guid AuthorId,
    Guid? AssigneeId,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? CompletedAt,
    uint Version,
    IReadOnlyList<TaskCommentItem> Comments,
    IReadOnlyList<TaskHistoryItem> History);

public sealed record TaskCommentItem(
    Guid CommentId,
    Guid AuthorId,
    string Text,
    DateTimeOffset CreatedAt);

public sealed record TaskHistoryItem(
    Guid HistoryId,
    Guid ChangedBy,
    TaskHistoryChangeType ChangeType,
    string? OldValue,
    string? NewValue,
    DateTimeOffset ChangedAt);
