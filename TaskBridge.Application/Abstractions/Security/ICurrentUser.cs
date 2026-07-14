namespace TaskBridge.Application.Abstractions.Security;

/// <summary>
/// Represents ICurrentUser.
/// </summary>
public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
}
