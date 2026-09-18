using TaskBridge.Application.Abstractions.Time;

namespace TaskBridge.IntegrationTests.Helpers;

public sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; }
}
