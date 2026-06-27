using System.Text;
using Mansari.Store.Users.Domain.Common;

namespace Mansari.Store.Users.Domain.ValueObjects;

public sealed class NationalCode : ValueObject
{
    public string Value { get; }

    private NationalCode(string value)
    {
        Value = value;
    }

    public static NationalCode Create(string value)
    {
        var normalized = Normalize(value);

        if (normalized.Length != 10 || !normalized.All(char.IsDigit))
            throw new DomainException("VALIDATION_ERROR", "National code must contain exactly 10 digits.");

        if (normalized.Distinct().Count() == 1)
            throw new DomainException("VALIDATION_ERROR", "National code is not valid.");

        var check = int.Parse(normalized[9].ToString());
        var sum = 0;

        for (var i = 0; i < 9; i++)
            sum += int.Parse(normalized[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var validation = remainder < 2 ? remainder : 11 - remainder;

        if (check != validation)
            throw new DomainException("VALIDATION_ERROR", "National code checksum is invalid.");

        return new NationalCode(normalized);
    }

    public static string Normalize(string value)
    {
        return NormalizeDigits(value).Trim();
    }

    private static string NormalizeDigits(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var ch in value)
        {
            if (ch is >= '۰' and <= '۹')
                builder.Append((char)('0' + (ch - '۰')));
            else if (ch is >= '٠' and <= '٩')
                builder.Append((char)('0' + (ch - '٠')));
            else
                builder.Append(ch);
        }

        return builder.ToString();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
