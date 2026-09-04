using AutoMapper;

using TaskBridge.Application.Comments.AddTaskComment;
using TaskBridge.Application.Projects.CreateProject;
using TaskBridge.Application.Tasks.ChangeTaskStatus;
using TaskBridge.Application.Tasks.CreateTask;
using TaskBridge.Application.Tasks.GetProjectTasks;
using TaskBridge.Application.Tasks.GetTaskDetails;
using TaskBridge.Application.Workspaces.CreateWorkspace;
using TaskBridge.Contracts.Comments;
using TaskBridge.Contracts.History;
using TaskBridge.Contracts.Projects;
using TaskBridge.Contracts.Tasks;
using TaskBridge.Contracts.Workspace;

namespace TaskBridge.Api.Mapping;

public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<CreateWorkspaceResult, WorkspaceResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.WorkspaceId));

        CreateMap<CreateProjectResult, ProjectResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.ProjectId))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ArchivedAt,
                opt => opt.MapFrom(_ => (DateTimeOffset?)null));

        CreateMap<CreateTaskResult, TaskResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.CompletedAt,
                opt => opt.MapFrom(_ => (DateTimeOffset?)null));

        CreateMap<ChangeTaskStatusResult, ChangeTaskStatusResponse>()
            .ForMember(dest => dest.TaskId,
                opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<ProjectTaskListItem, TaskListItemResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => src.Priority.ToString()));

        CreateMap<GetTaskDetailsResult, TaskListItemResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.TaskId));

        CreateMap<TaskCommentItem, TaskCommentResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.CommentId))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<TaskHistoryItem, TaskHistoryResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.HistoryId))
            .ForMember(dest => dest.ChangeType,
                opt => opt.MapFrom(src => src.ChangeType.ToString()));

        CreateMap<AddTaskCommentResult, TaskCommentResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.CommentId))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));
    }
}
