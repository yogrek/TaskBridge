using System.Security.Claims;

using TaskBridge.Application.Abstractions.Security;

namespace TaskBridge.Api.CurrentUser;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out var parsed) ? parsed : Guid.Empty;
        }
    }
}
