namespace TicketTracker.Api.Services.Auth;

public class AuthResult
{
    public bool Success { get; }
    public AuthErrorCode ErrorCode { get; }
    public string? ErrorMessage { get; }

    protected AuthResult(bool success, AuthErrorCode errorCode, string? errorMessage)
    {
        Success = success;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static AuthResult Ok() => new(true, AuthErrorCode.None, null);

    public static AuthResult Fail(AuthErrorCode errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);
}

public class AuthResult<T> : AuthResult
{
    public T? Value { get; }

    private AuthResult(bool success, T? value, AuthErrorCode errorCode, string? errorMessage)
        : base(success, errorCode, errorMessage)
    {
        Value = value;
    }

    public static AuthResult<T> Ok(T value) => new(true, value, AuthErrorCode.None, null);

    public static new AuthResult<T> Fail(AuthErrorCode errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage);
}
