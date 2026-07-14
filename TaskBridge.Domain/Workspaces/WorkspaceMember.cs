namespace TaskBridge.Domain.Workspaces;

/// <summary>
/// Связь пользователя и рабочего пространства
/// </summary>
public sealed class WorkspaceMember
{
    public Guid Id { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public WorkspaceRole Role { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }

    private WorkspaceMember()
    {
        // EF Core
    }

    public WorkspaceMember(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role,
        DateTimeOffset joinedAt)
    {
        Id = Guid.NewGuid();
        WorkspaceId = workspaceId;
        UserId = userId;
        Role = role;
        JoinedAt = joinedAt;
    }

    public void ChangeRole(WorkspaceRole newRole) =>
        Role = newRole;
}
