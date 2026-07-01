using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketTracker.Api.Contracts.Auth;
using TicketTracker.Api.Data;
using TicketTracker.Api.Entities;
using TicketTracker.Api.Options;

namespace TicketTracker.Api.Services.Auth;

public class AuthService : IAuthService
{
    private const int VerificationTokenValidityHours = 24;

    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailSender _emailSender;
    private readonly AppOptions _appOptions;

    public AuthService(
        AppDbContext db,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailSender emailSender,
        IOptions<AppOptions> appOptions)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailSender = emailSender;
        _appOptions = appOptions.Value;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var alreadyExists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (alreadyExists)
        {
            return AuthResult.Fail(AuthErrorCode.EmailAlreadyRegistered, "A user with this email is already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            DisplayName = request.DisplayName.Trim(),
            IsEmailVerified = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Users.Add(user);

        var verificationToken = CreateVerificationToken(user.Id);
        _db.EmailVerificationTokens.Add(verificationToken);

        await _db.SaveChangesAsync(cancellationToken);

        await SendVerificationEmailAsync(user, verificationToken, cancellationToken);

        return AuthResult.Ok();
    }

    public async Task<AuthResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return AuthResult<LoginResponse>.Fail(AuthErrorCode.InvalidCredentials, "Invalid email or password.");
        }

        if (!user.IsEmailVerified)
        {
            return AuthResult<LoginResponse>.Fail(AuthErrorCode.EmailNotVerified, "Email address has not been verified.");
        }

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return AuthResult<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        });
    }

    public async Task<AuthResult> VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        var verificationToken = await _db.EmailVerificationTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.Token == token, cancellationToken);

        if (verificationToken is null || verificationToken.IsUsed || verificationToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return AuthResult.Fail(AuthErrorCode.InvalidOrExpiredToken, "Verification token is invalid or has expired.");
        }

        verificationToken.IsUsed = true;
        verificationToken.User.IsEmailVerified = true;

        await _db.SaveChangesAsync(cancellationToken);

        return AuthResult.Ok();
    }

    public async Task<AuthResult> ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // Respond identically regardless of whether the account exists or is already verified,
        // so this endpoint can't be used to enumerate registered emails.
        if (user is null || user.IsEmailVerified)
        {
            return AuthResult.Ok();
        }

        var previousTokens = await _db.EmailVerificationTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var previousToken in previousTokens)
        {
            previousToken.IsUsed = true;
        }

        var verificationToken = CreateVerificationToken(user.Id);
        _db.EmailVerificationTokens.Add(verificationToken);

        await _db.SaveChangesAsync(cancellationToken);

        await SendVerificationEmailAsync(user, verificationToken, cancellationToken);

        return AuthResult.Ok();
    }

    private static EmailVerificationToken CreateVerificationToken(Guid userId)
    {
        var now = DateTimeOffset.UtcNow;

        return new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = GenerateSecureToken(),
            CreatedAt = now,
            ExpiresAt = now.AddHours(VerificationTokenValidityHours),
            IsUsed = false
        };
    }

    private static string GenerateSecureToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    private async Task SendVerificationEmailAsync(User user, EmailVerificationToken token, CancellationToken cancellationToken)
    {
        var verificationLink = $"{_appOptions.PublicUrl.TrimEnd('/')}/api/auth/verify-email?token={token.Token}";

        await _emailSender.SendAsync(
            user.Email,
            "Verify your email address",
            $"Hi {user.DisplayName},\n\nPlease verify your email address by visiting the link below:\n{verificationLink}\n\nThis link expires in {VerificationTokenValidityHours} hours.",
            cancellationToken);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
