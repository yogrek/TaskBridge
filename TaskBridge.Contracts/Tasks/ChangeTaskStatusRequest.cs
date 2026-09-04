namespace TaskBridge.Contracts.Tasks;

public sealed record ChangeTaskStatusRequest(
    string Status,
    uint ExpectedVersion);
