using TaskBridge.Application.Abstractions.Security;

namespace TaskBridge.IntegrationTests.Helpers;

public sealed class FakeCurrentUser : ICurrentUser
{
    public Guid UserId { get; set; }

    public bool IsAuthenticated { get; set; } = true;
}
