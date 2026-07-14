using Microsoft.Extensions.DependencyInjection;

using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Comments.AddTaskComment;
using TaskBridge.Application.Projects.CreateProject;
using TaskBridge.Application.Security;
using TaskBridge.Application.Tasks.ChangeTaskStatus;
using TaskBridge.Application.Tasks.CreateTask;
using TaskBridge.Application.Tasks.GetProjectTasks;
using TaskBridge.Application.Tasks.GetTaskDetails;
using TaskBridge.Application.Workspaces.CreateWorkspace;

namespace TaskBridge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<CreateWorkspaceHandler>();
        services.AddScoped<CreateProjectHandler>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<ChangeTaskStatusHandler>();
        services.AddScoped<AddTaskCommentHandler>();
        services.AddScoped<GetProjectTasksHandler>();
        services.AddScoped<GetTaskDetailsHandler>();

        return services;
    }
}
