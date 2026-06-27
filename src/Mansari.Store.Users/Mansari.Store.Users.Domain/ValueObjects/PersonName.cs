using System.Text.RegularExpressions;
using Mansari.Store.Users.Domain.Common;

namespace Mansari.Store.Users.Domain.ValueObjects;

public sealed class PersonName : ValueObject
{
    private static readonly Regex NameRegex = new(@"^[\p{L}\p{M} ._\-'’]{2,100}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private PersonName(string value)
    {
        Value = value;
    }

    public static PersonName Create(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("VALIDATION_ERROR", $"{fieldName} cannot be empty.");

        var normalized = value.Trim();

        if (normalized.Length is < 2 or > 100)
            throw new DomainException("VALIDATION_ERROR", $"{fieldName} must be between 2 and 100 characters.");

        if (!NameRegex.IsMatch(normalized))
            throw new DomainException("VALIDATION_ERROR", $"{fieldName} contains invalid characters.");

        return new PersonName(normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
