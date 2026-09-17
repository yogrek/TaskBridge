namespace TaskBridge.Application.Abstractions.Security;

/// <summary>
/// Represents IPasswordHasher.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
