namespace TaskBridge.Contracts.Authentification;

public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    DateTimeOffset ExpiresAt);
