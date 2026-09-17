using TaskBridge.Domain.Users;

namespace TaskBridge.Application.Abstractions.Security;

/// <summary>
/// Represents ITokenProvider.
/// </summary>
public interface IAccessTokenProvider
{
    AccessTokenResult Create(User user);
}

public sealed record AccessTokenResult(
    string AccessToken,
    DateTimeOffset ExpiresAt);
