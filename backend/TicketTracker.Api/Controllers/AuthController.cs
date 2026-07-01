using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketTracker.Api.Contracts.Auth;
using TicketTracker.Api.Services.Auth;

namespace TicketTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(new { message = "Registration successful. Please check your email to verify your account." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token, CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyEmailAsync(token, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(new { message = "Email verified successfully." });
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendVerificationAsync(request, cancellationToken);
        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(new { message = "If an account with that email exists and is not yet verified, a new verification email has been sent." });
    }

    private IActionResult MapError(AuthResult result)
    {
        var statusCode = result.ErrorCode switch
        {
            AuthErrorCode.EmailAlreadyRegistered => StatusCodes.Status409Conflict,
            AuthErrorCode.InvalidCredentials => StatusCodes.Status401Unauthorized,
            AuthErrorCode.EmailNotVerified => StatusCodes.Status403Forbidden,
            AuthErrorCode.InvalidOrExpiredToken => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(statusCode, new { message = result.ErrorMessage });
    }
}
