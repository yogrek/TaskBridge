using Microsoft.EntityFrameworkCore;

using TaskBridge.Application.Abstractions.Persistence;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.Application.Common;

namespace TaskBridge.Application.Authentification.Login;

public sealed class LoginHandler
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenProvider _tokenProvider;

    public LoginHandler(
        IAppDbContext context,
        IPasswordHasher passwordHasher,
        IAccessTokenProvider tokenProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<Result<LoginResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (user is null)
            return InvalidCredentials();

        if (!user.IsActive)
            return InvalidCredentials();

        var passwordValid = _passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!passwordValid)
            return InvalidCredentials();

        var token = _tokenProvider.Create(user);

        return Result<LoginResult>.Success(
            new LoginResult(
                user.Id,
                user.Email,
                user.DisplayName,
                token.AccessToken,
                token.ExpiresAt));
    }

    private static Result<LoginResult> InvalidCredentials() =>
        Result<LoginResult>.Failure(
            Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password"));
}
