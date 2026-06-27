using Mansari.Store.Catalog.Domain.Common;

namespace Mansari.Store.Catalog.Domain.ValueObjects;

public sealed class StockQuantity : ValueObject
{
    public int Value { get; private set; }

    private StockQuantity() { }

    private StockQuantity(int value)
    {
        Value = value;
    }

    public static StockQuantity Create(int value)
    {
        if (value < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        return new StockQuantity(value);
    }

    public bool HasEnough(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Requested quantity must be greater than zero.");

        return Value >= quantity;
    }

    public StockQuantity Increase(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to increase must be greater than zero.");

        return Create(Value + quantity);
    }

    public StockQuantity Decrease(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        if (Value < quantity)
        {
            throw new InvalidOperationException("Insufficient stock.");
        }

        Value -= quantity;

        return this;
    }


    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

