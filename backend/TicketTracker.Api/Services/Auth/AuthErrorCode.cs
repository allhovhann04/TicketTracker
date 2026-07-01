namespace TicketTracker.Api.Services.Auth;

public enum AuthErrorCode
{
    None,
    EmailAlreadyRegistered,
    InvalidCredentials,
    EmailNotVerified,
    InvalidOrExpiredToken
}
