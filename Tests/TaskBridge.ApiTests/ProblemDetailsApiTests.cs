using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TaskBridge.ApiTests.Infrastructure;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Contracts.Authentification;
using TaskBridge.Domain.Users;

namespace TaskBridge.ApiTests;

public sealed class ProblemDetailsApiTests : ApiTestBase
{
    public ProblemDetailsApiTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Register_UnexpectedException_ShouldReturnSafeProblemDetails()
    {
        // Arrange
        using var factory = new TaskBridgeApiFactory(
            Fixture.ConnectionString,
            services =>
            {
                services.RemoveAll<IAccessTokenProvider>();
                services.AddScoped<IAccessTokenProvider, ThrowingAccessTokenProvider>();
            });
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("user@test.com", "Password123!", "User"));
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("Internal server error", body, StringComparison.Ordinal);
        Assert.DoesNotContain("InvalidOperationException", body, StringComparison.Ordinal);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ThrowingAccessTokenProvider : IAccessTokenProvider
    {
        public AccessTokenResult Create(User user) => throw new InvalidOperationException("Secret test exception");
    }
}
