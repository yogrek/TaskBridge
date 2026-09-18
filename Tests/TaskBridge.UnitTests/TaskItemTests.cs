using TaskBridge.Domain.Common;
using TaskBridge.Domain.Tasks;

using TaskStatus = TaskBridge.Domain.Tasks.TaskStatus;

namespace TaskBridge.UnitTests;

public class TaskItemTests
{
    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowDomainException()
    {
        // Act
        var action = () => CreateTask(title: " ");

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldSetStatusToNew()
    {
        // Act
        var task = CreateTask();

        // Assert
        Assert.Equal(TaskStatus.New, task.Status);
    }

    [Fact]
    public void ChangeStatus_ToDone_ShouldSetCompletedAt()
    {
        // Arrange
        var task = CreateTask();
        var completedAt = new DateTimeOffset(2026, 9, 2, 10, 0, 0, TimeSpan.Zero);

        // Act
        task.ChangeStatus(TaskStatus.Done, completedAt);

        // Assert
        Assert.Equal(TaskStatus.Done, task.Status);
        Assert.Equal(completedAt, task.CompletedAt);
        Assert.Equal(completedAt, task.UpdatedAt);
    }

    [Fact]
    public void ChangeStatus_FromDone_ShouldClearCompletedAt()
    {
        // Arrange
        var task = CreateTask();

        // Act
        task.ChangeStatus(TaskStatus.Done, task.CreatedAt.AddHours(1));
        task.ChangeStatus(TaskStatus.InProgress, task.CreatedAt.AddHours(2));

        // Assert
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void AssignTo_ShouldChangeAssigneeId()
    {
        // Arrange
        var task = CreateTask();
        var assigneeId = Guid.NewGuid();

        // Act
        task.AssignTo(assigneeId, DateTimeOffset.UtcNow);

        // Assert
        Assert.Equal(assigneeId, task.AssigneeId);
    }

    [Fact]
    public void Unassign_ShouldClearAssigneeId()
    {
        // Arrange
        var task = CreateTask(assigneeId: Guid.NewGuid());

        // Act
        task.Unassign(DateTimeOffset.UtcNow);

        // Assert
        Assert.Null(task.AssigneeId);
    }

    [Fact]
    public void ChangePriority_ShouldChangePriority()
    {
        // Arrange
        var task = CreateTask();

        // Act
        task.ChangePriority(TaskPriority.High, DateTimeOffset.UtcNow);

        // Assert
        Assert.Equal(TaskPriority.High, task.Priority);
    }

    [Fact]
    public void ChangeDueDate_ShouldChangeDueDate()
    {
        // Arrange
        var task = CreateTask();
        var dueDate = new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        task.ChangeDueDate(dueDate, DateTimeOffset.UtcNow);

        // Assert
        Assert.Equal(dueDate, task.DueDate);
    }

    private static TaskItem CreateTask(
        string title = "Test task",
        Guid? assigneeId = null)
    {
        return new TaskItem(
            Guid.NewGuid(),
            title,
            null,
            Guid.NewGuid(),
            assigneeId,
            TaskPriority.Normal,
            null,
            new DateTimeOffset(2026, 9, 1, 10, 0, 0, TimeSpan.Zero));
    }
}
