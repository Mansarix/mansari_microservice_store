
using Mansari.Store.Catalog.Domain.Common;

namespace Mansari.Store.Catalog.Domain.ValueObjects;


public sealed class AuthorName : ValueObject
{
    public const int MaxLength = 150;

    public string Value { get; private set; } = default!;

    private AuthorName() { }

    private AuthorName(string value)
    {
        Value = value;
    }

    public static AuthorName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Author name cannot be empty.");

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new DomainException($"Author name cannot exceed {MaxLength} characters.");

        return new AuthorName(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }

    public override string ToString()
    {
        return Value;
    }
}

