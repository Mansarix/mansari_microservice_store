using System.Net.Mail;
using Mansari.Store.Users.Domain.Common;

namespace Mansari.Store.Users.Domain.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress Create(string value)
    {
        var normalized = Normalize(value);

        try
        {
            _ = new MailAddress(normalized);
        }
        catch
        {
            throw new DomainException("VALIDATION_ERROR", "Email is invalid.");
        }

        return new EmailAddress(normalized);
    }

    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("VALIDATION_ERROR", "Email cannot be empty.");

        return value.Trim().ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
