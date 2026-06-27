using Mansari.Store.Ordering.Api.Contracts;
using Mansari.Store.Ordering.Application.Orders.Commands.CreateOrder;
using Mansari.Store.Ordering.Domain.Orders.Entities;

namespace Mansari.Store.Ordering.Api.Mappings;

public static class OrderMappings
{
    public static CreateOrderCommand ToCommand(this CreateOrderRequest request)
        => new CreateOrderCommand(request.BookId, request.Quantity);

    public static CreateOrderResponse ToResponse(
    this Order order)
    {
        return new CreateOrderResponse(
            order.Id,
            order.BookId,
            order.Quantity,
            order.Status.ToString(),
            order.CreatedAtUtc);
    }
}