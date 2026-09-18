using TaskBridge.Domain.Common;
using TaskBridge.Domain.Workspaces;

namespace TaskBridge.UnitTests;

public class WorkspaceTests
{
    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        // Act
        var action = () => CreateWorkspace(name: " ");

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Rename_WithValidName_ShouldChangeName()
    {
        // Arrange
        var workspace = CreateWorkspace();

        // Act
        workspace.Rename("Renamed workspace");

        // Assert
        Assert.Equal("Renamed workspace", workspace.Name);
    }

    private static Workspace CreateWorkspace(string name = "Test workspace")
    {
        return new Workspace(
            name,
            Guid.NewGuid(),
            new DateTimeOffset(2026, 9, 1, 10, 0, 0, TimeSpan.Zero));
    }
}
