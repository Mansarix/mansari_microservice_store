using Mansari.Store.Gateway.Extensions;
using Mansari.Store.Gateway.Services.Abstractions;
using Mansari.Store.Gateway.Services.Aggregation;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BasketController : ControllerBase
{
    private readonly IBasketAggregationService _basketAggregationService;

    public BasketController(
        IBasketAggregationService basketAggregationService)
    {
        _basketAggregationService = basketAggregationService;
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetBasket(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _basketAggregationService
            .GetBasketAsync(userId, cancellationToken);

        return result.ToActionResult();
    }
}