using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Common;

namespace TaskBridge.Application.Tasks.GetProjectTasks;

/// <summary>
/// Represents GetProjectTasksHandler.
/// </summary>
public class GetProjectTasksHandler
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetProjectTasksHandler(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ProjectTaskListItem>>> Handle(
        GetProjectTasksQuery query,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var project = await _context.Projects
            .FirstOrDefaultAsync(x => x.Id == query.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result<PagedResult<ProjectTaskListItem>>.Failure(
                Error.NotFound("Project.NotFound", "Project was not found."));
        }

        var isMember = await _context.WorkspaceMembers
            .AnyAsync(
                x => x.WorkspaceId == project.WorkspaceId &&
                     x.UserId == _currentUser.UserId,
                cancellationToken);

        if (!isMember)
        {
            return Result<PagedResult<ProjectTaskListItem>>.Failure(
                Error.Forbidden("Project.AccessForbidden", "User has no access to this project."));
        }

        var tasksQuery = _context.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == query.ProjectId);

        if (query.Status is not null)
            tasksQuery = tasksQuery.Where(x => x.Status == query.Status.Value);

        if (query.AssigneeId is not null)
            tasksQuery = tasksQuery.Where(x => x.AssigneeId == query.AssigneeId.Value);

        var totalCount = await tasksQuery.CountAsync(cancellationToken);

        var items = await tasksQuery
            .OrderByDescending(x => x.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProjectTaskListItem(
                x.Id,
                x.Title,
                x.Status,
                x.Priority,
                x.AssigneeId,
                x.DueDate,
                x.UpdatedAt,
                x.Version))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ProjectTaskListItem>(
            items,
            page,
            pageSize,
            totalCount);

        return Result<PagedResult<ProjectTaskListItem>>.Success(result);
    }
}
