namespace TaskBridge.Application.Authentification.Login;

public sealed record LoginCommand(
    string Email,
    string Password);
