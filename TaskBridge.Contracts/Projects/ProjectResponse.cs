namespace TaskBridge.Contracts.Projects;

public sealed record ProjectResponse(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string? Descriprion,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ArchivedAt);
