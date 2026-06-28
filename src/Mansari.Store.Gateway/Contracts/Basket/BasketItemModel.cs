namespace Mansari.Store.Gateway.Contracts.Basket;

public sealed class BasketItemModel
{
    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}
