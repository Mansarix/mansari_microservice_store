using System.Text;
using System.Text.RegularExpressions;
using Mansari.Store.Users.Domain.Common;

namespace Mansari.Store.Users.Domain.ValueObjects;

public sealed class MobileNumber : ValueObject
{
    private static readonly Regex MobileRegex = new(@"^09\d{9}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private MobileNumber(string value)
    {
        Value = value;
    }

    public static MobileNumber Create(string value)
    {
        var normalized = Normalize(value);

        if (!MobileRegex.IsMatch(normalized))
            throw new DomainException("VALIDATION_ERROR", "Mobile number is invalid.");

        return new MobileNumber(normalized);
    }

    public static string Normalize(string value)
    {
        var digitsOnly = NormalizeDigits(value);

        if (digitsOnly.StartsWith("0098", StringComparison.Ordinal) && digitsOnly.Length == 14)
            digitsOnly = "0" + digitsOnly[4..];
        else if (digitsOnly.StartsWith("98", StringComparison.Ordinal) && digitsOnly.Length == 12)
            digitsOnly = "0" + digitsOnly[2..];
        else if (digitsOnly.StartsWith("98", StringComparison.Ordinal) && digitsOnly.Length == 13)
            digitsOnly = "0" + digitsOnly[3..];

        return digitsOnly;
    }

    private static string NormalizeDigits(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var ch in value)
        {
            if (char.IsDigit(ch))
                builder.Append(ch);
            else if (ch is >= '۰' and <= '۹')
                builder.Append((char)('0' + (ch - '۰')));
            else if (ch is >= '٠' and <= '٩')
                builder.Append((char)('0' + (ch - '٠')));
        }

        return builder.ToString();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
