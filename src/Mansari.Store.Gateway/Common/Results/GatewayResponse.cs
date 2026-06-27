namespace Mansari.Store.Gateway.Common.Results;

//من عاشق این گیتوی ریزالت شدم! اگه درکش بکنی عمقشو تو هم عاشقش میشی
// خیلی وقت گرفت 40 خط کد شاید به اندازه یک میکروسرویس کامل وقت گرفت و شاید 8 بار از اول طراحیش کردم تا به این نسخه رسید
//ساده نگذر ازش ، برو تو عمقش ، لذت ببر 
// یک باندد کانتکست تشخیص داده شده! یک زبان مشترک! این ریزالت تمام خروجی ها را هندل میکند
public sealed class GatewayResult<T>
{
    public bool IsSuccess { get; }

    public T? Value { get; }

    public GatewayError? Error { get; }

    private GatewayResult(
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

    internal static GatewayResult<T> Success(T value)
        => new(
            true,
            value,
            null);

    internal static GatewayResult<T> Failure(GatewayError error)
        => new(
            false,
            default,
            error);
}