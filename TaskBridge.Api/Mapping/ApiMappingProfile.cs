using AutoMapper;

using TaskBridge.Application.Authentification.Login;
using TaskBridge.Application.Authentification.Register;
using TaskBridge.Application.Comments.AddTaskComment;
using TaskBridge.Application.Projects.CreateProject;
using TaskBridge.Application.Tasks.ChangeTaskStatus;
using TaskBridge.Application.Tasks.CreateTask;
using TaskBridge.Application.Tasks.GetProjectTasks;
using TaskBridge.Application.Tasks.GetTaskDetails;
using TaskBridge.Application.Workspaces.CreateWorkspace;
using TaskBridge.Contracts.Authentification;
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
        CreateMap<CreateWorkspaceResult, WorkspaceResponse>(MemberList.None)
            .ConstructUsing(src => new WorkspaceResponse(
                src.WorkspaceId,
                src.Name,
                src.OwnerId,
                src.CreatedAt));

        CreateMap<CreateProjectResult, ProjectResponse>(MemberList.None)
            .ConstructUsing(src => new ProjectResponse(
                src.ProjectId,
                src.WorkspaceId,
                src.Name,
                src.Description,
                src.Status.ToString(),
                src.CreatedAt,
                null));

        CreateMap<CreateTaskResult, TaskResponse>(MemberList.None)
            .ConstructUsing(src => new TaskResponse(
                src.TaskId,
                src.ProjectId,
                src.Title,
                src.Description,
                src.Status.ToString(),
                src.Priority.ToString(),
                src.AuthorId,
                src.AssigneeId,
                src.DueDate,
                src.CreatedAt,
                src.UpdatedAt,
                src.CompletedAt,
                src.Version));

        CreateMap<ChangeTaskStatusResult, ChangeTaskStatusResponse>()
            .ForMember(dest => dest.TaskId,
                opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<ProjectTaskListItem, TaskListItemResponse>(MemberList.None)
            .ConstructUsing(src => new TaskListItemResponse(
                src.TaskId,
                src.Title,
                src.Status.ToString(),
                src.Priority.ToString(),
                src.AssigneeId,
                src.DueDate,
                src.UpdatedAt,
                src.Version));

        CreateMap<GetTaskDetailsResult, TaskDetailsResponse>(MemberList.None)
            .ConstructUsing(src => new TaskDetailsResponse(
                src.TaskId,
                src.ProjectId,
                src.Title,
                src.Description,
                src.Status.ToString(),
                src.Priority.ToString(),
                src.AuthorId,
                src.AssigneeId,
                src.DueDate,
                src.CreatedAt,
                src.UpdatedAt,
                src.CompletedAt,
                src.Version,
                src.Comments.Select(comment => new TaskCommentResponse(
                    comment.CommentId,
                    comment.TaskId,
                    comment.AuthorId,
                    comment.Text,
                    comment.CreatedAt,
                    comment.CreatedAt)).ToList(),
                src.History.Select(history => new TaskHistoryResponse(
                    history.HistoryId,
                    history.TaskId,
                    history.ChangedBy,
                    history.ChangeType.ToString(),
                    history.OldValue,
                    history.NewValue,
                    history.ChangedAt)).ToList()));

        CreateMap<TaskCommentItem, TaskCommentResponse>(MemberList.None)
            .ConstructUsing(src => new TaskCommentResponse(
                src.CommentId,
                src.TaskId,
                src.AuthorId,
                src.Text,
                src.CreatedAt,
                src.CreatedAt));

        CreateMap<TaskHistoryItem, TaskHistoryResponse>(MemberList.None)
            .ConstructUsing(src => new TaskHistoryResponse(
                src.HistoryId,
                src.TaskId,
                src.ChangedBy,
                src.ChangeType.ToString(),
                src.OldValue,
                src.NewValue,
                src.ChangedAt));

        CreateMap<AddTaskCommentResult, TaskCommentResponse>(MemberList.None)
            .ConstructUsing(src => new TaskCommentResponse(
                src.CommentId,
                src.TaskId,
                src.AuthorId,
                src.Text,
                src.CreatedAt,
                src.CreatedAt));

        CreateMap<RegisterResult, AuthResponse>();

        CreateMap<LoginResult, AuthResponse>();
    }
}
