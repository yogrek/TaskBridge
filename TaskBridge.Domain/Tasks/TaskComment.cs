namespace TaskBridge.Domain.Tasks;

/// <summary>
/// Комментарий пользователя к задаче
/// </summary>
public sealed class TaskComment
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Text { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private TaskComment()
    {
        // EF Core
    }

    public TaskComment(
        Guid taskId,
        Guid authorId,
        string text,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        AuthorId = authorId;
        Text = text.Trim();
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void Edit(string text, DateTimeOffset changedAt)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException("Comment text cannot be empty");

        Text = text.Trim();
        UpdatedAt = changedAt;
    }
}
