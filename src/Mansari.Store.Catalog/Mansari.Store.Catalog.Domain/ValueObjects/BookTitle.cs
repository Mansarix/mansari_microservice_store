using Mansari.Store.Catalog.Domain.Common;

namespace Mansari.Store.Catalog.Domain.ValueObjects;


public sealed class BookTitle : ValueObject
{
    public const int MaxLength = 200;

    public string Value { get; private set; } = default!;

    private BookTitle() { }

    private BookTitle(string value)
    {
        Value = value;
    }

    public static BookTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Book title cannot be empty.");

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new DomainException($"Book title cannot exceed {MaxLength} characters.");

        return new BookTitle(value);
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
