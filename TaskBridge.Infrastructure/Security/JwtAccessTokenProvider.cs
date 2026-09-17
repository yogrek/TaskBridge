using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Domain.Users;

namespace TaskBridge.Infrastructure.Security;

public sealed class JwtAccessTokenProvider : IAccessTokenProvider
{
    private readonly JwtOptions _options;

    public JwtAccessTokenProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessTokenResult Create(User user)
    {
        var now = DateTimeOffset.UtcNow;

        var expiresAt = now.AddMinutes(_options.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(tokenValue, expiresAt);
    }
}
