namespace Mansari.Store.Users.Domain.Common;

public sealed class DomainException : Exception
{
    public string ErrorCode { get; }

    public DomainException(string message)
        : this("DOMAIN_ERROR", message)
    {
    }

    public DomainException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
