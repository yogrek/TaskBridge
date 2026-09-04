using TaskBridge.Contracts.Comments;
using TaskBridge.Contracts.History;

namespace TaskBridge.Contracts.Tasks;

public sealed record TaskDetailsResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    Guid AuthorId,
    Guid? AssigneeId,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? CompletedAt,
    uint Version,
    IReadOnlyList<TaskCommentResponse> Comments,
    IReadOnlyList<TaskHistoryResponse> History);
