using TaskBridge.Domain.Common;

namespace TaskBridge.Domain.Workspaces;
/// <summary>
/// Рабочее пространство - объединяет проекты, участников и задачи
/// </summary>
public sealed class Workspace
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid OwnerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Workspace()
    {
        // EF Core
    }

    public Workspace(
        string name,
        Guid ownerId,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Workspace name cannot be empty");

        Id = Guid.NewGuid();
        Name = name.Trim();
        OwnerId = ownerId;
        CreatedAt = createdAt;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Workspace name cannot be empty");

        Name = newName.Trim();
    }
}
