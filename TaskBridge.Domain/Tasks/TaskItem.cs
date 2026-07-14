using TaskBridge.Domain.Common;

namespace TaskBridge.Domain.Tasks;

/// <summary>
/// Сущность задачи
/// </summary>
public sealed class TaskItem
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }

    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }

    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }

    public Guid AuthorId { get; private set; }
    public Guid? AssigneeId { get; private set; }

    public DateTimeOffset? DueDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public uint Version { get; private set; }

    private TaskItem()
    {
        // EF Core
    }

    public TaskItem(
        Guid projectId,
        string title,
        string? description,
        Guid authorId,
        Guid? assigneeId,
        TaskPriority priority,
        DateTimeOffset? dueDate,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title cannot be empty");

        Id = Guid.NewGuid();
        ProjectId = projectId;
        Title = title;
        Description = description;
        AuthorId = authorId;
        AssigneeId = assigneeId;
        Priority = priority;
        DueDate = dueDate;
        Status = TaskStatus.New;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public void Rename(string title, DateTimeOffset changedAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title cannot be empty");

        Title = title.Trim();
        UpdatedAt = changedAt;
    }

    public void ChangeDescription(string description, DateTimeOffset changedAt)
    {
        Description = description;
        UpdatedAt = changedAt;
    }

    public void ChangeStatus(TaskStatus newStatus,  DateTimeOffset changedAt)
    {
        if (Status == newStatus)
            return;

        Status = newStatus;
        UpdatedAt = changedAt;

        if (Status == TaskStatus.Done)
            CompletedAt = changedAt;
        else
            CompletedAt = null;
    }

    public void ChangePriority(TaskPriority newPriority, DateTimeOffset changedAt)
    {
        Priority = newPriority;
        UpdatedAt = changedAt;
    }

    public void AssignTo(Guid assigneeId, DateTimeOffset changedAt)
    {
        AssigneeId = assigneeId;
        UpdatedAt = changedAt;
    }

    public void Unassign(DateTimeOffset changedAt)
    {
        AssigneeId = null;
        UpdatedAt = changedAt;
    }

    public void ChangeDueDate(DateTimeOffset? dueDate,  DateTimeOffset changedAt)
    {
        DueDate = dueDate;
        UpdatedAt = changedAt;
    }
}
