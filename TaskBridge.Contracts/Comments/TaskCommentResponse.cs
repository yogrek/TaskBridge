namespace TaskBridge.Contracts.Comments;

public sealed record TaskCommentResponse(
    Guid Id,
    Guid TaskId,
    Guid AuthorId,
    string Text,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
