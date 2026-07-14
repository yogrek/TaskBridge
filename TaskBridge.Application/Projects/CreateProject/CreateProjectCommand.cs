namespace TaskBridge.Application.Projects.CreateProject;

/// <summary>
/// Represents CreateProjectCommand.
/// </summary>
public sealed record CreateProjectCommand(
    Guid WorkspaceId,
    string Name,
    string? Description);
