namespace TaskBridge.Contracts.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    Guid? AssigneeId,
    string Priority,
    DateTimeOffset? DueDate);
