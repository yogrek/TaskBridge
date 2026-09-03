using System.Net.NetworkInformation;

using TaskBridge.Domain.Common;

namespace TaskBridge.Domain.Projects;

/// <summary>
/// Проект внутри рабочего пространства
/// </summary>
public sealed class Project
{
    public Guid Id { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public ProjectStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ArchivedAt { get; private set; }

    public bool IsArchived => Status is ProjectStatus.Archived;

    private Project()
    {
        // EF Core
    }

    public Project(
        Guid workspaceId,
        string name,
        string? description,
        DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        WorkspaceId = workspaceId;
        Name = name.Trim();
        Description = description;
        Status = ProjectStatus.Active;
        CreatedAt = createdAt;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Project name cannot be empty");

        Name = newName.Trim();
    }

    public void Archive(DateTimeOffset archivedAt)
    {
        if (Status == ProjectStatus.Archived)
            return;

        Status = ProjectStatus.Archived;
        ArchivedAt = archivedAt;
    }

    public void Complete()
    {
        if (Status is ProjectStatus.Archived)
            throw new DomainException("Archived project cannot be completed.");

        Status = ProjectStatus.Completed;
    }
}
