namespace TaskBridge.Domain.Tasks;

public enum TaskHistoryChangeType
{
    TaskCreated = 1,
    TitleChanged = 2,
    DescriptionChanged = 3,
    StatusChanged = 4,
    AssigneeChanged = 5,
    DueDateChanged = 6,
    PriorityChanged = 7,
    CommentAdded = 8,
    CommentEdited = 9,
}
