using Mansari.Store.Gateway.Contracts.Basket;

namespace Mansari.Store.Gateway.GrpcClients.Abstractions;

public interface IBasketGrpcClient
{
    Task<BasketModel> GetBasketAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
