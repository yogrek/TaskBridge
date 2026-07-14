using TaskBridge.Domain.Projects;

namespace TaskBridge.Application.Projects.CreateProject;

/// <summary>
/// Represents CreateProjectResult.
/// </summary>
public sealed record CreateProjectResult(
    Guid ProjectId,
    Guid WorkspaceId,
    string Name,
    string? Description,
    ProjectStatus Status,
    DateTimeOffset CreatedAt);
