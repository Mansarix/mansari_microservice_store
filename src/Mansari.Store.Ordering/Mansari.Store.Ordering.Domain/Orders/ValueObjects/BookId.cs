using Mansari.Store.Ordering.Domain.Common;

namespace Mansari.Store.Ordering.Domain.Orders.ValueObjects;

public sealed class BookId : ValueObject
{
    public Guid Value { get; }

    private BookId(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("BookId cannot be empty.");

        Value = value;
    }

    public static BookId Create(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(BookId id) => id.Value;
}
