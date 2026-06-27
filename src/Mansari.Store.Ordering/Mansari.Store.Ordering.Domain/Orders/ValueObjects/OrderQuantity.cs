using Mansari.Store.Ordering.Domain.Common;

namespace Mansari.Store.Ordering.Domain.Orders.ValueObjects;

public sealed class OrderQuantity : ValueObject
{
    public int Value { get; }

    private OrderQuantity(int value)
    {
        if (value <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (value > 1000)
            throw new DomainException("Quantity exceeds allowed limit.");

        Value = value;
    }

    public static OrderQuantity Create(int value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator int(OrderQuantity quantity) => quantity.Value;
}
