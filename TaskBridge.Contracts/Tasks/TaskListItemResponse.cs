namespace TaskBridge.Contracts.Tasks;

public sealed record TaskListItemResponse(
    Guid Id,
    string Title,
    string Status,
    string Priority,
    Guid? AssigneeId,
    DateTimeOffset? DueDate,
    DateTimeOffset UpdatedAt,
    uint Version);
