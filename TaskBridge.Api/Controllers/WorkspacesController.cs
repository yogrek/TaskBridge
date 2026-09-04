using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBridge.Api.Extensions;
using TaskBridge.Application.Workspaces.CreateWorkspace;
using TaskBridge.Contracts.Workspace;

namespace TaskBridge.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workspaces")]
public sealed class WorkspacesController : ControllerBase
{
    private readonly CreateWorkspaceHandler _createWorkspaceHandler;
    private readonly IMapper _mapper;

    public WorkspacesController(CreateWorkspaceHandler createWorkspaceHandler, IMapper mapper)
    {
        _createWorkspaceHandler = createWorkspaceHandler;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(WorkspaceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkspaceResponse>> CreateWorkspace(CreateWorkspaceRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateWorkspaceCommand(request.Name);

        var result = await _createWorkspaceHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.ToActionResult(this);

        var response = _mapper.Map<WorkspaceResponse>(result.Value);

        return Created($"/api/workspaces/{response.Id}", response);
    }
}
