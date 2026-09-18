using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using TaskBridge.ApiTests.Infrastructure;
using TaskBridge.Contracts.Authentification;

namespace TaskBridge.ApiTests;

public sealed class AuthenticationApiTests : ApiTestBase
{
    public AuthenticationApiTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Register_ValidRequest_ShouldReturnAccessToken()
    {
        // Act
        var response = await Factory.CreateClient().PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("user@test.com", "Password123!", "User"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.Equal("user@test.com", auth.Email);
        Assert.False(string.IsNullOrWhiteSpace(auth.AccessToken));
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturnConflictProblemDetails()
    {
        // Arrange
        var client = Factory.CreateClient();
        var request = new RegisterRequest("user@test.com", "Password123!", "User");
        await client.PostAsJsonAsync("/api/auth/register", request);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemDetailsAsync(response, HttpStatusCode.Conflict, "Conflict", "Auth.EmailAlreadyExists");
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnAccessToken()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("user@test.com", "Password123!", "User"));

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("user@test.com", "Password123!"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.AccessToken));
    }

    [Theory]
    [InlineData("wrong-password")]
    [InlineData("missing-user")]
    public async Task Login_InvalidCredentials_ShouldReturnUnauthorized(string scenario)
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("user@test.com", "Password123!", "User"));
        var request = scenario == "wrong-password"
            ? new LoginRequest("user@test.com", "IncorrectPassword!")
            : new LoginRequest("missing@test.com", "Password123!");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized, "Unauthorized", "Auth.InvalidCredentials");
    }

    private static async Task AssertProblemDetailsAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedTitle,
        string expectedType)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal((int)expectedStatus, problem.Status);
        Assert.Equal(expectedTitle, problem.Title);
        Assert.Equal(expectedType, problem.Type);
        Assert.False(string.IsNullOrWhiteSpace(problem.Detail));
        Assert.True(problem.Extensions.TryGetValue("traceId", out var traceId));
        Assert.NotNull(traceId);
    }
}
