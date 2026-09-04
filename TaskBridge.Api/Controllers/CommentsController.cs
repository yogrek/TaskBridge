using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBridge.Api.Extensions;
using TaskBridge.Application.Comments.AddTaskComment;
using TaskBridge.Contracts.Comments;

namespace TaskBridge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks/{taskId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly AddTaskCommentHandler _addTaskCommentHandler;
    private readonly IMapper _mapper;

    public CommentsController(AddTaskCommentHandler addTaskCommentHandler, IMapper mapper)
    {
        _addTaskCommentHandler = addTaskCommentHandler;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskCommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TaskCommentResponse>> AddComment(
        Guid taskId,
        AddTaskCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTaskCommentCommand(taskId, request.Text);

        var result = await _addTaskCommentHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = _mapper.Map<TaskCommentResponse>(result.Value);

        return Created($"/api/tasks/{taskId}/comments/{response.Id}", response);
    }
}
