namespace TaskBridge.Application.Authentification.Register;

public sealed record RegisterResult(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    DateTimeOffset ExpiresAt);
