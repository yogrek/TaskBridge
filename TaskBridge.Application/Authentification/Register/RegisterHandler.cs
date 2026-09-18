using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Abstractions.Time;
using TaskBridge.Application.Common;
using TaskBridge.Domain.Users;

namespace TaskBridge.Application.Authentification.Register;

public sealed class RegisterHandler
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenProvider _tokenProvider;
    private readonly IClock _clock;

    public RegisterHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        IAccessTokenProvider tokenProvider,
        IClock clock)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _clock = clock;
    }

    public async Task<Result<RegisterResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result<RegisterResult>.Failure(Error.Validation(
                "Auth.EmailRequred",
                "Email is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result<RegisterResult>.Failure(Error.Validation(
                "Auth.PasswordRequired",
                "Password is required."));
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var userExist = await _context.Users
            .AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
        if (userExist)
        {
            return Result<RegisterResult>.Failure(Error.Conflict(
                "Auth.EmailAlreadyExists",
                "User with this email already exists."));
        }

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = new User(
            command.Email,
            command.DisplayName,
            passwordHash,
            _clock.UtcNow);

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenProvider.Create(user);

        return Result<RegisterResult>.Success(
            new RegisterResult(
                user.Id,
                user.Email,
                user.DisplayName,
                token.AccessToken,
                token.ExpiresAt));
    }
}
