namespace TaskBridge.Contracts.Tasks;

public sealed record AssignTaskRequest(
    Guid AssigneeId,
    uint ExpectedVersion);
