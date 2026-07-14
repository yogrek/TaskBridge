namespace TaskBridge.Domain.Tasks;

public enum TaskStatus
{
    New = 1,
    InProgress = 2,
    Blocked = 3,
    Review = 4,
    Done = 5,
    Cancelled = 6,
}
