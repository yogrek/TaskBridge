using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc;

using TaskBridge.ApiTests.Infrastructure;
using TaskBridge.Contracts.Tasks;

namespace TaskBridge.ApiTests;

public sealed class TasksApiTests : ApiTestBase
{
    public TasksApiTests(PostgreSqlFixture fixture) : base(fixture) { }

    [Fact]
    public async Task GetTask_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Factory.CreateClient().GetAsync($"/api/tasks/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTask_ExpiredOrInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            TestJwtTokenFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddMinutes(-2)));

        // Act
        var expiredResponse = await client.GetAsync($"/api/tasks/{Guid.NewGuid()}");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            TestJwtTokenFactory.Create(Guid.NewGuid(), signingKey: "incorrect-test-signing-key-with-sufficient-length"));
        var invalidSignatureResponse = await client.GetAsync($"/api/tasks/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, expiredResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, invalidSignatureResponse.StatusCode);
    }

    [Fact]
    public async Task ChangeTaskStatus_DifferentWorkspace_ShouldReturnForbidden()
    {
        // Arrange
        var data = await ApiTestData.CreateWorkspaceTaskAsync(Fixture.ConnectionString, includeSecondWorkspace: true);
        var client = CreateAuthorizedClient(data.Outsider!.Id);

        // Act
        var response = await client.PatchAsJsonAsync(
            $"/api/tasks/{data.Task.Id}/status",
            new ChangeTaskStatusRequest("Done", data.Task.Version));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await AssertProblemDetailsAsync(response, HttpStatusCode.Forbidden, "Forbidden", "Task.ChangeStatusForbidden");
    }

    [Fact]
    public async Task GetTask_MissingTask_ShouldReturnNotFound()
    {
        // Act
        var response = await CreateAuthorizedClient(Guid.NewGuid()).GetAsync($"/api/tasks/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound, "Not found", "Task.NotFound");
    }

    [Fact]
    public async Task ChangeTaskStatus_InvalidStatus_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var data = await ApiTestData.CreateWorkspaceTaskAsync(Fixture.ConnectionString);

        // Act
        var response = await CreateAuthorizedClient(data.Owner.Id).PatchAsJsonAsync(
            $"/api/tasks/{data.Task.Id}/status",
            new ChangeTaskStatusRequest("NotAStatus", data.Task.Version));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest, "Validation error", "Task.InvalidStatus");
    }

    [Fact]
    public async Task ChangeTaskStatus_StaleVersion_ShouldReturnConflictProblemDetails()
    {
        // Arrange
        var data = await ApiTestData.CreateWorkspaceTaskAsync(Fixture.ConnectionString);

        // Act
        var response = await CreateAuthorizedClient(data.Owner.Id).PatchAsJsonAsync(
            $"/api/tasks/{data.Task.Id}/status",
            new ChangeTaskStatusRequest("Done", data.Task.Version + 1));

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemDetailsAsync(response, HttpStatusCode.Conflict, "Conflict", "Task.VersionConflict");
    }

    private HttpClient CreateAuthorizedClient(Guid userId)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            TestJwtTokenFactory.Create(userId));

        return client;
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
