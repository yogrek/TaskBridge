namespace TaskBridge.Contracts.Authentification;

public sealed record LoginRequest(
    string Email,
    string Password);
