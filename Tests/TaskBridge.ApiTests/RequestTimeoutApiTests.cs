using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using TaskBridge.ApiTests.Infrastructure;

namespace TaskBridge.ApiTests;

public sealed class RequestTimeoutApiTests
{
    [Fact]
    public async Task SlowEndpoint_WhenRequestTimesOut_ShouldReturnGatewayTimeout()
    {
        using var factory = new TaskBridgeApiFactory(
            "Host=127.0.0.1;Port=1;Database=taskbridge_timeout_tests;Username=taskbridge;Password=taskbridge",
            services =>
            {
                services.AddControllers().AddApplicationPart(typeof(SlowTestController).Assembly);
                services.PostConfigure<RequestTimeoutOptions>(options =>
                {
                    options.DefaultPolicy = new RequestTimeoutPolicy
                    {
                        Timeout = TimeSpan.FromMilliseconds(100),
                        TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
                    };
                });
            });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/test/slow");

        Assert.Equal(HttpStatusCode.GatewayTimeout, response.StatusCode);
    }
}

[ApiController]
[Route("api/test/slow")]
public sealed class SlowTestController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        return Ok();
    }
}
