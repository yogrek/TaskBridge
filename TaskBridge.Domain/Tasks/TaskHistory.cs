namespace TaskBridge.Domain.Tasks;

/// <summary>
/// Запись истории изменения задачи
/// </summary>
public sealed class TaskHistory
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid ChangedBy { get; private set; }
    public TaskHistoryChangeType ChangeType { get; private set; }
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }

    private TaskHistory()
    {
        // EF Core
    }

    public TaskHistory(
        Guid taskId,
        Guid changedBy,
        TaskHistoryChangeType changeType,
        string? oldValue,
        string? newValue,
        DateTimeOffset changedAt)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        ChangedBy = changedBy;
        ChangeType = changeType;
        OldValue = oldValue;
        NewValue = newValue;
        ChangedAt = changedAt;
    }
}
