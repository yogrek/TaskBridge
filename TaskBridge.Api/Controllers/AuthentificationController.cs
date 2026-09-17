using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBridge.Api.Extensions;
using TaskBridge.Application.Authentification.Login;
using TaskBridge.Application.Authentification.Register;
using TaskBridge.Contracts.Authentification;

namespace TaskBridge.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthentificationController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly LoginHandler _loginHandler;
    private readonly IMapper _mapper;

    public AuthentificationController(
        RegisterHandler registerHandler,
        LoginHandler loginHandler,
        IMapper mapper)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.DisplayName);

        var result = await _registerHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult(this);

        return Ok(_mapper.Map<AuthResponse>(result.Value));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await _loginHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult(this);

        return Ok(_mapper.Map<AuthResponse>(result.Value));
    }
}
