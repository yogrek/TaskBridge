namespace TaskBridge.Application.Authentification.Login;

public sealed record LoginResult(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    DateTimeOffset ExpiresAt);
