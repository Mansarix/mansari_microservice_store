
using Mansari.Store.Catalog.Domain.Common;

namespace Mansari.Store.Catalog.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = default!;

    private Money() { }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "IRR")
    {
        if (amount < 0)
            throw new DomainException("Price cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency cannot be empty.");

        currency = currency.Trim().ToUpperInvariant();

        if (currency.Length != 3)
            throw new DomainException("Currency must be a 3-letter code.");

        return new Money(amount, currency);
    }

    public Money ChangeAmount(decimal amount)
    {
        return Create(amount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString()
    {
        return $"{Amount} {Currency}";
    }
}

