namespace TaskBridge.Contracts.History;

public sealed record TaskHistoryResponse(
    Guid Id,
    Guid TaskId,
    Guid ChangedBy,
    string ChangeType,
    string? OldValue,
    string? NewValue,
    DateTimeOffset ChangedAt);
