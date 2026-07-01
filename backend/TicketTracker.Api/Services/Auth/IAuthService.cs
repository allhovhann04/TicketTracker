using TicketTracker.Api.Contracts.Auth;

namespace TicketTracker.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);
    Task<AuthResult> ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default);
}
