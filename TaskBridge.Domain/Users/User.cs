using TaskBridge.Domain.Common;

namespace TaskBridge.Domain.Users;

/// <summary>
/// Пользователь системы
/// </summary>
public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private User()
    {
        // EF Core
    }

    public User(
        string email,
        string displayName,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAt = createdAt;
    }

    public void Activate() =>
        IsActive = true;

    public void Deactivate() =>
        IsActive = false;

    public void ChangeDisplayName(string newDisplayName)
    {
        if (string.IsNullOrWhiteSpace(newDisplayName))
            throw new DomainException("User display name cannot be empty");

        DisplayName = newDisplayName.Trim();
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("User password cannot be empty");

        PasswordHash = newPasswordHash;
    }
}
