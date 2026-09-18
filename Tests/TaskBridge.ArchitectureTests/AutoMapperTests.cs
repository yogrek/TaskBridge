using AutoMapper;

using Microsoft.Extensions.Logging.Abstractions;

using TaskBridge.Api.Mapping;

namespace TaskBridge.ArchitectureTests;

public sealed class AutoMapperTests
{
    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<ApiMappingProfile>(),
            NullLoggerFactory.Instance);

        // Act / Assert
        configuration.AssertConfigurationIsValid();
    }
}
