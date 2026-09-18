using TaskBridge.Domain.Common;
using TaskBridge.Domain.Projects;

namespace TaskBridge.UnitTests;

public class ProjectTests
{
    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Act
        var action = () => CreateProject(name: " ");

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldSetStatusToActive()
    {
        // Act
        var project = CreateProject();

        // Assert
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public void Archive_ShouldSetStatusAndArchivedAt()
    {
        // Arrange
        var project = CreateProject();
        var archivedAt = new DateTimeOffset(2026, 9, 2, 10, 0, 0, TimeSpan.Zero);

        // Act
        project.Archive(archivedAt);

        // Assert
        Assert.Equal(ProjectStatus.Archived, project.Status);
        Assert.Equal(archivedAt, project.ArchivedAt);
    }

    [Fact]
    public void Complete_WhenArchived_ShouldThrowDomainException()
    {
        // Arrange
        var project = CreateProject();
        project.Archive(new DateTimeOffset(2026, 9, 2, 10, 0, 0, TimeSpan.Zero));

        // Act
        var action = project.Complete;

        // Assert
        Assert.Throws<DomainException>(action);
    }

    private static Project CreateProject(string name = "Test project")
    {
        return new Project(
            Guid.NewGuid(),
            name,
            null,
            new DateTimeOffset(2026, 9, 1, 10, 0, 0, TimeSpan.Zero));
    }
}
