namespace TaskBridge.Application.Abstractions.Security;

/// <summary>
/// Represents IPermissionService.
/// </summary>
public interface IPermissionService
{
    Task<bool> CanCreateProjectAsync(Guid userId, Guid wodkspaceId, CancellationToken cancellationToken);
    Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId, CancellationToken cancellationToken);
    Task<bool> CanAssignTaskAsync(Guid userId, Guid projectId, Guid assigneeId, CancellationToken cancellationToken);
    Task<bool> CanChangeTaskStatusAsync(Guid userId, Guid taskId, CancellationToken cancellationToken);
    Task<bool> CanAddCommentAsync(Guid userId, Guid taskId, CancellationToken cancellationToken);
}
