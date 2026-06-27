namespace Mansari.Store.Users.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }

    protected Result(bool isSuccess, string errorCode, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public static Result Success()
        => new(true, string.Empty, string.Empty);

    public static Result Failure(string errorCode, string errorMessage)
        => new(false, errorCode, errorMessage);
}

public sealed class Result<T> : Result
{
    public T Value { get; }

    private Result(T value, bool isSuccess, string errorCode, string errorMessage)
        : base(isSuccess, errorCode, errorMessage)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new(value, true, string.Empty, string.Empty);

    public static Result<T> Failure(string errorCode, string errorMessage)
        => new(default!, false, errorCode, errorMessage);
}
