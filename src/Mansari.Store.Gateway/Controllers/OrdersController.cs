using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.Gateway.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderDetailsService _service;

    public OrdersController(OrderDetailsService service)
    {
        _service = service;
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetOrderDetails(Guid id)
    {
        var result = await _service.GetOrderDetails(id);
        return Ok(result);
    }
}
