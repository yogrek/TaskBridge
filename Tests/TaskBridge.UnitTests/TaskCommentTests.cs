using TaskBridge.Domain.Common;
using TaskBridge.Domain.Tasks;

namespace TaskBridge.UnitTests;

public class TaskCommentTests
{
    [Fact]
    public void Create_WithEmptyText_ShouldThrowDomainException()
    {
        // Act
        var action = () => CreateComment(text: " ");

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Edit_ShouldChangeTextAndUpdatedAt()
    {
        // Arrange
        var comment = CreateComment();
        var updatedAt = new DateTimeOffset(2026, 9, 1, 11, 0, 0, TimeSpan.Zero);

        // Act
        comment.Edit("Updated comment", updatedAt);

        // Assert
        Assert.Equal("Updated comment", comment.Text);
        Assert.Equal(updatedAt, comment.UpdatedAt);
    }

    private static TaskComment CreateComment(string text = "Test comment")
    {
        var createdAt = new DateTimeOffset(2026, 9, 1, 10, 0, 0, TimeSpan.Zero);

        return new TaskComment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            text,
            createdAt,
            createdAt);
    }
}
