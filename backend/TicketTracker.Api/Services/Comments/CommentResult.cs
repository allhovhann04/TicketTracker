namespace TicketTracker.Api.Services.Comments;

public class CommentResult
{
    public bool Success { get; }
    public CommentErrorCode ErrorCode { get; }
    public string? ErrorMessage { get; }

    protected CommentResult(bool success, CommentErrorCode errorCode, string? errorMessage)
    {
        Success = success;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static CommentResult Ok() => new(true, CommentErrorCode.None, null);

    public static CommentResult Fail(CommentErrorCode errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);
}

public class CommentResult<T> : CommentResult
{
    public T? Value { get; }

    private CommentResult(bool success, T? value, CommentErrorCode errorCode, string? errorMessage)
        : base(success, errorCode, errorMessage)
    {
        Value = value;
    }

    public static CommentResult<T> Ok(T value) => new(true, value, CommentErrorCode.None, null);

    public static new CommentResult<T> Fail(CommentErrorCode errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage);
}
