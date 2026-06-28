namespace Mansari.Store.Gateway.Common.Results;

public sealed class GatewayResult<T>
{
    public bool IsSuccess { get; }

    public T? Value { get; }

    public GatewayError? Error { get; }

    internal GatewayResult(
        bool isSuccess,
        T? value,
        GatewayError? error)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException(
                "A successful result cannot contain an error.");
        }

        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException(
                "A failed result must contain an error.");
        }

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
}

public static class GatewayResult
{
    public static GatewayResult<T> Success<T>(T value)
        => new(
            true,
            value,
            null);

    public static GatewayResult<T> Failure<T>(GatewayError error)
        => new(
            false,
            default,
            error);
}