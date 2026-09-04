namespace TaskBridge.Contracts.Tasks;

public sealed record ChangeTaskStatusResponse(
    Guid TaskId,
    string Status,
    DateTimeOffset? CompletedAt,
    DateTimeOffset UpdatedAt,
    uint Version);
