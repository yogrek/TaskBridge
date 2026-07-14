namespace TaskBridge.Application.Abstractions.Time;

/// <summary>
/// Represents IClock.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
