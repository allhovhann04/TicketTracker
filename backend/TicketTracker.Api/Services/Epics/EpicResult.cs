namespace TicketTracker.Api.Services.Epics;

public class EpicResult
{
    public bool Success { get; }
    public EpicErrorCode ErrorCode { get; }
    public string? ErrorMessage { get; }

    protected EpicResult(bool success, EpicErrorCode errorCode, string? errorMessage)
    {
        Success = success;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static EpicResult Ok() => new(true, EpicErrorCode.None, null);

    public static EpicResult Fail(EpicErrorCode errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);
}

public class EpicResult<T> : EpicResult
{
    public T? Value { get; }

    private EpicResult(bool success, T? value, EpicErrorCode errorCode, string? errorMessage)
        : base(success, errorCode, errorMessage)
    {
        Value = value;
    }

    public static EpicResult<T> Ok(T value) => new(true, value, EpicErrorCode.None, null);

    public static new EpicResult<T> Fail(EpicErrorCode errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage);
}
