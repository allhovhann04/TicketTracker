namespace TicketTracker.Api.Services.Tickets;

public class TicketResult
{
    public bool Success { get; }
    public TicketErrorCode ErrorCode { get; }
    public string? ErrorMessage { get; }

    protected TicketResult(bool success, TicketErrorCode errorCode, string? errorMessage)
    {
        Success = success;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static TicketResult Ok() => new(true, TicketErrorCode.None, null);

    public static TicketResult Fail(TicketErrorCode errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);
}

public class TicketResult<T> : TicketResult
{
    public T? Value { get; }

    private TicketResult(bool success, T? value, TicketErrorCode errorCode, string? errorMessage)
        : base(success, errorCode, errorMessage)
    {
        Value = value;
    }

    public static TicketResult<T> Ok(T value) => new(true, value, TicketErrorCode.None, null);

    public static new TicketResult<T> Fail(TicketErrorCode errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage);
}
