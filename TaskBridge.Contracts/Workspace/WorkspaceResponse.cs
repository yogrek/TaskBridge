namespace TaskBridge.Contracts.Workspace;

public sealed record WorkspaceResponse(
    Guid Id,
    string Name,
    Guid OwnerId,
    DateTimeOffset CreatedAt);
