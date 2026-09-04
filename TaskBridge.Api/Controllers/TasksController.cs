using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBridge.Api.Extensions;
using TaskBridge.Application.Tasks.ChangeTaskStatus;
using TaskBridge.Application.Tasks.CreateTask;
using TaskBridge.Application.Tasks.GetProjectTasks;
using TaskBridge.Application.Tasks.GetTaskDetails;
using TaskBridge.Contracts.Common;
using TaskBridge.Contracts.Tasks;
using TaskBridge.Domain.Tasks;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public sealed class TasksController : ControllerBase
{
    private readonly CreateTaskHandler _createTaskHandler;
    private readonly ChangeTaskStatusHandler _changeTaskStatusHandler;
    private readonly GetProjectTasksHandler _getProjectTasksHandler;
    private readonly GetTaskDetailsHandler _getTaskDetailsHandler;
    private readonly IMapper _mapper;

    public TasksController(
        CreateTaskHandler createTaskHandler,
        ChangeTaskStatusHandler changeTaskStatusHandler,
        GetProjectTasksHandler getProjectTasksHandler,
        GetTaskDetailsHandler getTaskDetailsHandler,
        IMapper mapper)
    {
        _createTaskHandler = createTaskHandler;
        _changeTaskStatusHandler = changeTaskStatusHandler;
        _getProjectTasksHandler = getProjectTasksHandler;
        _getTaskDetailsHandler = getTaskDetailsHandler;
        _mapper = mapper;
    }

    [HttpPost("projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]

    public async Task<ActionResult<TaskResponse>> CreateTask(
        Guid projectId,
        CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<TaskPriority>(request.Priority, ignoreCase: true, out var priority))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "Invalid task priority",
                Status = StatusCodes.Status400BadRequest,
                Type = "Task.InvalidPriority",
                Instance = HttpContext.Request.Path
            });
        }

        var command = new CreateTaskCommand(
            projectId,
            request.Title,
            request.Description,
            request.AssigneeId,
            priority,
            request.DueDate);

        var result = await _createTaskHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = _mapper.Map<TaskResponse>(result.Value);

        return Created($"/api/tasks/{response.Id}", response);
    }

    [HttpGet("project/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(PagedResponse<TaskListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<TaskListItemResponse>>> GetProjectTasks(
        Guid projectId,
        [FromQuery] string? status,
        [FromQuery] Guid? assigneeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        TaskStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<TaskStatus>(status, ignoreCase: true, out var parsed))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Validation error",
                    Detail = "Invalid task status",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "Task.InvalidStatus",
                    Instance = HttpContext.Request.Path
                });
            }

            parsedStatus = parsed;
        }

        var query = new GetProjectTasksQuery(
            projectId,
            parsedStatus,
            assigneeId,
            page,
            pageSize);

        var result = await _getProjectTasksHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = result.Value!.ToPagedResponse<ProjectTaskListItem, TaskListItemResponse>(_mapper);

        return Ok(response);
    }

    [HttpGet("tasks/{taskId:guid}")]
    [ProducesResponseType(typeof(TaskDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsResponse>> GetTaskDetails(Guid taskId, CancellationToken cancellationToken)
    {
        var query = new GetTaskDetailsQuery(taskId);

        var result = await _getTaskDetailsHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = _mapper.Map<TaskDetailsResponse>(result.Value);

        return Ok(response);
    }

    [HttpPatch("tasks/{taskId:guid}/status")]
    [ProducesResponseType(typeof(ChangeTaskStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChangeTaskStatusResponse>> ChangeStatus(
        Guid taskId,
        ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var status))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "Invalid task status",
                Status = StatusCodes.Status400BadRequest,
                Type = "Task.InvalidStatus",
                Instance = HttpContext.Request.Path
            });
        }

        var command = new ChangeTaskStatusCommand(taskId, status, request.ExpectedVersion);

        var result = await _changeTaskStatusHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = _mapper.Map<ChangeTaskStatusResponse>(result.Value);

        return Ok(response);
    }
}
