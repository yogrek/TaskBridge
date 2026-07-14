using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Abstractions.Time;
using TaskBridge.Application.Common;
using TaskBridge.Domain.Workspaces;

namespace TaskBridge.Application.Workspaces.CreateWorkspace;

/// <summary>
/// Represents CreateWorkspaceHandler.
/// </summary>
public sealed class CreateWorkspaceHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClock _clock;

    public CreateWorkspaceHandler(
        IAppDbContext context,
        ICurrentUser currentUser,
        IClock clock)
    {
        _context = context;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<Result<CreateWorkspaceResult>> Handle(
        CreateWorkspaceCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            return Result<CreateWorkspaceResult>.Failure(
                Error.Unauthorized("User.Unauthorized", "User is not authenticated."));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<CreateWorkspaceResult>.Failure(
                Error.Validation("Workspace.NameRequired", "Workspace name is required."));

        var userExists = await _context
            .Users
            .AnyAsync(x => x.Id == _currentUser.UserId && x.IsActive, cancellationToken);

        if (!userExists)
        {
            return Result<CreateWorkspaceResult>.Failure(
                Error.NotFound("user.Notfound", "Current user was not found."));
        }

        var now = _clock.UtcNow;

        var workspace = new Workspace(command.Name, _currentUser.UserId, now);

        var member = new WorkspaceMember(workspace.Id, _currentUser.UserId, WorkspaceRole.Owner, now);

        _context.Workspaces.Add(workspace);
        _context.WorkspaceMembers.Add(member);

        await _context.SaveChangesAsync(cancellationToken);

        var result = new CreateWorkspaceResult(
            workspace.Id,
            workspace.Name,
            workspace.OwnerId,
            workspace.CreatedAt);

        return Result<CreateWorkspaceResult>.Success(result);
    }
}
