using TaskBridge.Application.Abstractions.Time;

namespace TaskBridge.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
