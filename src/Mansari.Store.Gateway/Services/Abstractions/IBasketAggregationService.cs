using Mansari.Store.Gateway.Common.Results;
using Mansari.Store.Gateway.Contracts.Basket;

namespace Mansari.Store.Gateway.Services.Abstractions;

public interface IBasketAggregationService
{
    Task<GatewayResult<BasketModel>> GetBasketAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}