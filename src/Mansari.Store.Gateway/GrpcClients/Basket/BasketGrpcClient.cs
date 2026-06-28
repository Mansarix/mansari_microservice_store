using Mansari.Store.Gateway.Contracts.Basket;
using Mansari.Store.Gateway.GrpcClients.Abstractions;
using Mansari.Store.Gateway.Services.Aggregation;

namespace Mansari.Store.Gateway.GrpcClients.Basket;

public sealed class BasketGrpcClient : IBasketGrpcClient
{
    private readonly BasketAggregationService _client;

    public BasketGrpcClient(
        BasketAggregationService client)
    {
        _client = client;
    }

    public async Task<BasketModel> GetBasketAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetBasketAsync(
             userId
            , cancellationToken: cancellationToken);

        return Map(response.Value);
    }

    private static BasketModel Map(
        BasketModel response)
    {
        return new BasketModel
        {
            UserId = response.UserId,
            Items = response.Items
                .Select(x => new BasketItemModel
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })
                .ToList()
        };
    }
}