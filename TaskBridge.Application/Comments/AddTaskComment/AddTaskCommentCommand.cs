namespace TaskBridge.Application.Comments.AddTaskComment;

/// <summary>
/// Represents AddTaskCommentCommand.
/// </summary>
public sealed record AddTaskCommentCommand(
    Guid TaskId,
    string Text);
