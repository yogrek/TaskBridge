using Microsoft.EntityFrameworkCore;

using TaskBridge.Domain.Projects;
using TaskBridge.Domain.Tasks;
using TaskBridge.Domain.Users;
using TaskBridge.Domain.Workspaces;

namespace TaskBridge.Application.Abstractions.Persistence;

/// <summary>
/// Represents IAppDbContext.
/// </summary>
public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkspaceMember> WorkspaceMembers { get; }
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<TaskComment> TaskComments { get; }
    DbSet<TaskHistory> TaskHistory { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
