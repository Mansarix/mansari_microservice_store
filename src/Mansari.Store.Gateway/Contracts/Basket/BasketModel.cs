namespace Mansari.Store.Gateway.Contracts.Basket;

public sealed class BasketModel
{
    public Guid UserId { get; init; }

    public IReadOnlyCollection<BasketItemModel> Items { get; init; }
        = [];
}
