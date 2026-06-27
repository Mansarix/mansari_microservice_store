using Mansari.Store.Contracts.Orders;
using Mansari.Store.Ordering.Application.Abstractions.Messaging;
using Mansari.Store.Ordering.Application.Common;
using Mansari.Store.Ordering.Application.DTOs;
using Mansari.Store.Ordering.Domain.Abstractions;
using Mansari.Store.Ordering.Domain.Orders.Entities;
using Mansari.Store.Ordering.Domain.Orders.Enums;
using Mansari.Store.Ordering.Domain.Orders.Interfaces;
using MediatR;

namespace Mansari.Store.Ordering.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly IOrderRepository _repository;
    private readonly IOutboxService _outboxService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IOutboxService outboxService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _outboxService = outboxService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateOrderResponse>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = Order.Create(
                request.BookId,
                request.Quantity);

            var orderCreatedEvent = new OrderCreatedEvent(
                order.Id,
                order.BookId,
                order.Quantity,
                Guid.NewGuid());

            await _repository.AddAsync(order, cancellationToken);

            _outboxService.Add(orderCreatedEvent);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CreateOrderResponse>.Success(new CreateOrderResponse(order.Id, order.Quantity,order.Status == OrderStatus.Pending, order.FailureReason));
        }
        catch (Exception ex)
        {
            return Result<CreateOrderResponse>.Failure(ex.Message);
        }
    }
}
