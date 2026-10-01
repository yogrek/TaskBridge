using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;
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
        using var client = factory.CreateClient();

        // Act
        using var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("user@test.com", "Password123!", "User"));
        var body = await response.Content.ReadAsStringAsync();
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Equal((int)HttpStatusCode.InternalServerError, problem.Status);
        Assert.Equal("Internal server error", problem.Title);
        Assert.Equal("/api/auth/register", problem.Instance);
        Assert.False(string.IsNullOrWhiteSpace(problem.Detail));
        Assert.True(problem.Extensions.TryGetValue("traceId", out var traceId));
        Assert.False(string.IsNullOrWhiteSpace(traceId?.ToString()));
        Assert.DoesNotContain("InvalidOperationException", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Secret test exception", body, StringComparison.Ordinal);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ThrowingAccessTokenProvider : IAccessTokenProvider
    {
        public AccessTokenResult Create(User user) => throw new InvalidOperationException("Secret test exception");
    }
}
