namespace TaskBridge.Application.Authentification.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string DisplayName);
