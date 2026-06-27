using Mansari.Store.Catalog.Domain.Common;
using Mansari.Store.Catalog.Domain.ValueObjects;

namespace Mansari.Store.Catalog.Domain.Entities;

public sealed class Book : Entity
{
    public BookTitle Title { get; private set; } = default!;
    public AuthorName Author { get; private set; } = default!;
    public StockQuantity Stock { get; private set; } = default!;
    public Money Price { get; private set; } = default!;

    private Book() {}

    private Book(
        Guid id,
        BookTitle title,
        AuthorName author,
        StockQuantity stock,
        Money price)
        : base(id)
    {
        Title = title;
        Author = author;
        Stock = stock;
        Price = price;
    }

    public static Book Create(
        string title,
        string author,
        int stock,
        decimal price,
        string currency = "IRR")
    {
        return new Book(
            Guid.NewGuid(),
            BookTitle.Create(title),
            AuthorName.Create(author),
            StockQuantity.Create(stock),
            Money.Create(price, currency));
    }

    public void UpdateDetails(
        string title,
        string author,
        decimal price,
        string currency = "IRR")
    {
        Title = BookTitle.Create(title);
        Author = AuthorName.Create(author);
        Price = Money.Create(price, currency);

        MarkAsUpdated();
    }

    public void ChangePrice(decimal price, string currency = "IRR")
    {
        Price = Money.Create(price, currency);

        MarkAsUpdated();
    }

    public void IncreaseStock(int quantity)
    {
        Stock = Stock.Increase(quantity);

        MarkAsUpdated();
    }

    public void DecreaseStock(int quantity)
    {
        Stock = Stock.Decrease(quantity);

        MarkAsUpdated();
    }

    public bool HasEnoughStock(int quantity)
    {
        return Stock.HasEnough(quantity);
    }

    public bool TryReserveStock(int quantity)
    {
        if (!Stock.HasEnough(quantity))
            return false;

        Stock = Stock.Decrease(quantity);

        MarkAsUpdated();

        return true;
    }
}
