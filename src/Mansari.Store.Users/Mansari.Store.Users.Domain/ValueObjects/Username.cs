using System.Text.RegularExpressions;
using Mansari.Store.Users.Domain.Common;

namespace Mansari.Store.Users.Domain.ValueObjects;

public sealed class Username : ValueObject
{
    private static readonly Regex UsernameRegex = new(@"^[a-z][a-z0-9_.]{3,31}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string value)
    {
        var normalized = Normalize(value);

        if (!UsernameRegex.IsMatch(normalized))
            throw new DomainException("VALIDATION_ERROR", "Username must start with a letter and contain 4 to 32 valid characters.");

        return new Username(normalized);
    }

    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("VALIDATION_ERROR", "Username cannot be empty.");

        return value.Trim().ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
