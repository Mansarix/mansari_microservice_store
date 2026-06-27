namespace Mansari.Store.Gateway.Common.Results;

// همچنین عاشق این جوجه هم هستم، اینم بخشی از گیتوی ریزالت هست
public sealed class GatewayError
{
    public string Code { get; }

    public string Message { get; }

    private GatewayError(
        string code,
        string message)
    {
        Code = code;
        Message = message;
    }

    public static GatewayError NotFound(
        string resource,
        object id)
        => new(
            $"{resource}.NotFound",
            $"{resource} '{id}' was not found.");

    public static GatewayError Validation(
        string message)
        => new(
            "Validation.Error",
            message);

    public static GatewayError ServiceUnavailable(
        string service)
        => new(
            $"{service}.Unavailable",
            $"{service} service is unavailable.");

    public static GatewayError Unexpected(
        string message)
        => new(
            "Unexpected.Error",
            message);
}