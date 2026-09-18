using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace TaskBridge.ApiTests.Infrastructure;

public static class TestJwtTokenFactory
{
    public const string Issuer = "taskbridge-api-tests";
    public const string Audience = "taskbridge-api-tests";
    public const string SigningKey = "taskbridge-api-tests-signing-key-with-sufficient-length";

    public static string Create(Guid userId, DateTimeOffset? expiresAt = null, string? signingKey = null)
    {
        var now = DateTimeOffset.UtcNow;
        var expiration = expiresAt ?? now.AddMinutes(5);
        var notBefore = expiration <= now ? expiration.AddMinutes(-1) : now;
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey ?? SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, userId.ToString())],
            notBefore: notBefore.UtcDateTime,
            expires: expiration.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
