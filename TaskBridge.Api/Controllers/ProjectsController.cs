using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBridge.Api.Extensions;
using TaskBridge.Application.Projects.CreateProject;
using TaskBridge.Contracts.Projects;

namespace TaskBridge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public sealed class ProjectsController : ControllerBase
{
    private readonly CreateProjectHandler _createProjectHandler;
    private readonly IMapper _mapper;

    public ProjectsController(CreateProjectHandler createProjectHandler, IMapper mapper)
    {
        _createProjectHandler = createProjectHandler;
        _mapper = mapper;
    }

    [HttpPost("workspaces/{workspaceId:guid}/projects")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjectResponse>> CreateProject(
        Guid workspaceId,
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectCommand(workspaceId, request.Name, request.Description);

        var result = await _createProjectHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = _mapper.Map<ProjectResponse>(result.Value);

        return Created($"/api/project/{response.Id}", response);
    }
}
