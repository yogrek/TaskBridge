namespace TaskBridge.Contracts.Authentification;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string DisplayName);
