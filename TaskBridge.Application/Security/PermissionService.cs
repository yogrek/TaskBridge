using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;

namespace TaskBridge.Application.Security;

public sealed class PermissionService : IPermissionService
{
    private readonly IAppDbContext _context;

    public PermissionService(IAppDbContext context) => _context = context;

    public Task<bool> CanCreateProjectAsync(Guid userId, Guid wodkspaceId, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<bool> CanAssignTaskAsync(Guid userId, Guid projectId, Guid assigneeId, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<bool> CanChangeTaskStatusAsync(Guid userId, Guid taskId, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<bool> CanAddCommentAsync(Guid userId, Guid taskId, CancellationToken cancellationToken) => throw new NotImplementedException();
}
