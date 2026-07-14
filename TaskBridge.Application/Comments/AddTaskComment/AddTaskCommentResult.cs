namespace TaskBridge.Application.Comments.AddTaskComment;

/// <summary>
/// Represents AddTaskCommentResult.
/// </summary>
public sealed record AddTaskCommentResult(
    Guid CommentId,
    Guid TaskId,
    Guid AuthorId,
    string Text,
    DateTimeOffset CreatedAt);
