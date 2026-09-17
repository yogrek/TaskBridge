using Microsoft.AspNetCore.Identity;

using TaskBridge.Application.Abstractions.Security;

namespace TaskBridge.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<PasswordHashContext> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(PasswordHashContext.Instance, password);

    public bool Verify(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(PasswordHashContext.Instance, passwordHash, password);

        return result != PasswordVerificationResult.Failed;
    }

    private sealed class PasswordHashContext
    {
        public static readonly PasswordHashContext Instance = new();
    }
}
