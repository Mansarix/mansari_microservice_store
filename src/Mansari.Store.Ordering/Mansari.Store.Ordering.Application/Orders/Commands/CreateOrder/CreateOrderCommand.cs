using Mansari.Store.Ordering.Application.Common;
using Mansari.Store.Ordering.Application.DTOs;
using MediatR;

namespace Mansari.Store.Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid BookId, int Quantity)
    : IRequest<Result<CreateOrderResponse>>;
