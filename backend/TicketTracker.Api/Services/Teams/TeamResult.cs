namespace TicketTracker.Api.Services.Teams;

public class TeamResult
{
    public bool Success { get; }
    public TeamErrorCode ErrorCode { get; }
    public string? ErrorMessage { get; }

    protected TeamResult(bool success, TeamErrorCode errorCode, string? errorMessage)
    {
        Success = success;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static TeamResult Ok() => new(true, TeamErrorCode.None, null);

    public static TeamResult Fail(TeamErrorCode errorCode, string errorMessage) =>
        new(false, errorCode, errorMessage);
}

public class TeamResult<T> : TeamResult
{
    public T? Value { get; }

    private TeamResult(bool success, T? value, TeamErrorCode errorCode, string? errorMessage)
        : base(success, errorCode, errorMessage)
    {
        Value = value;
    }

    public static TeamResult<T> Ok(T value) => new(true, value, TeamErrorCode.None, null);

    public static new TeamResult<T> Fail(TeamErrorCode errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage);
}
