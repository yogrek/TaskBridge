namespace TaskBridge.Application.Workspaces.CreateWorkspace;

/// <summary>
/// Represents CreateWorkspaceResult.
/// </summary>
public sealed record CreateWorkspaceResult(
    Guid WorkspaceId,
    string Name,
    Guid OwnerId,
    DateTimeOffset CreatedAt);
