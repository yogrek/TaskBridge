using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Abstractions.Time;
using TaskBridge.Application.Common;
using TaskBridge.Domain.Projects;

namespace TaskBridge.Application.Projects.CreateProject;

/// <summary>
/// Represents CreateProjectHandler.
/// </summary>
public sealed class CreateProjectHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPermissionService _permissionService;
    private readonly IClock _clock;

    public CreateProjectHandler(
        IAppDbContext context,
        ICurrentUser currentUser,
        IPermissionService permissionService,
        IClock clock)
    {
        _context = context;
        _currentUser = currentUser;
        _permissionService = permissionService;
        _clock = clock;
    }

    public async Task<Result<CreateProjectResult>> Handle(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<CreateProjectResult>.Failure(
                Error.Validation("Project.NameRequired", "Project name is required"));
        }

        var workspaceExist = await _context.Workspaces
            .AnyAsync(x => x.Id == command.WorkspaceId, cancellationToken);

        if (!workspaceExist)
        {
            return Result<CreateProjectResult>.Failure(
                Error.NotFound("Workspace.NotFound", "Workspace was not found"));
        }

        var canCreate = await _permissionService.CanCreateProjectAsync(
            _currentUser.UserId,
            command.WorkspaceId,
            cancellationToken);

        if (!canCreate)
        {
            return Result<CreateProjectResult>.Failure(
                Error.Forbidden("Project.CreateForbidden", "User cannot create project"));
        }

        var nameAlreadyExist = await _context.Projects
            .AnyAsync(x => x.WorkspaceId == command.WorkspaceId &&
                x.Name == command.Name.Trim(),
                cancellationToken);

        if (nameAlreadyExist)
        {
            return Result<CreateProjectResult>.Failure(
                Error.Conflict(
                    "Project.NameAlreadyExists",
                    "Project with the same name already exists in this workspace"));
        }

        var project = new Project(
            command.WorkspaceId,
            command.Name,
            command.Description,
            _clock.UtcNow);

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(cancellationToken);

        var result = new CreateProjectResult(
            project.Id,
            project.WorkspaceId,
            project.Name,
            project.Description,
            project.Status,
            project.CreatedAt);

        return Result<CreateProjectResult>.Success(result);
    }
}
