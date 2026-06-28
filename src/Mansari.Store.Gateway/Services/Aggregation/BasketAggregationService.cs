using Grpc.Core;
using Mansari.Store.Gateway.Common.Results;
using Mansari.Store.Gateway.Contracts.Basket;
using Mansari.Store.Gateway.GrpcClients.Abstractions;
using Mansari.Store.Gateway.Services.Abstractions;

namespace Mansari.Store.Gateway.Services.Aggregation;

public sealed class BasketAggregationService
    : IBasketAggregationService
{
    private readonly IBasketGrpcClient _basketGrpcClient;

    public BasketAggregationService(
        IBasketGrpcClient basketGrpcClient)
    {
        _basketGrpcClient = basketGrpcClient;
    }

    public async Task<GatewayResult<BasketModel>> GetBasketAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var basket = await _basketGrpcClient.GetBasketAsync(
                userId,
                cancellationToken);

            return GatewayResult.Success(basket);
        }
        catch (RpcException ex)
        {
            return ex.StatusCode switch
            {
                StatusCode.NotFound =>
                    GatewayResult.Failure<BasketModel>(
                        GatewayError.NotFound(
                            "Basket",
                            userId)),

                StatusCode.Unavailable =>
                    GatewayResult.Failure<BasketModel>(
                        GatewayError.ServiceUnavailable("Basket")),

                _ =>
                    GatewayResult.Failure<BasketModel>(
                        GatewayError.Unexpected(ex.Status.Detail))
            };
        }
        catch (Exception)
        {
            return GatewayResult.Failure<BasketModel>(
                GatewayError.Unexpected(
                    "Unexpected error while retrieving basket."));
        }
    }
}
