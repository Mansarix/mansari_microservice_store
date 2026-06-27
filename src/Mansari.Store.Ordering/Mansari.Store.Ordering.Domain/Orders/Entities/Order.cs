using Mansari.Store.Ordering.Domain.Common;
using Mansari.Store.Ordering.Domain.Orders.Enums;
using Mansari.Store.Ordering.Domain.Orders.ValueObjects;

namespace Mansari.Store.Ordering.Domain.Orders.Entities;

public sealed class Order : Entity, IAggregateRoot
{
    public BookId BookId { get; private set; } = default!;
    public OrderQuantity Quantity { get; private set; } = default!;

    public OrderStatus Status { get; private set; }

    public string? FailureReason { get; private set; }
    public DateTime? FailedAtUtc { get; private set; }
    public DateTime? ConfirmedAtUtc { get; private set; }

    private Order()  { }

    private Order(Guid id, BookId bookId, OrderQuantity quantity)
        : base(id)
    {
        BookId = bookId;
        Quantity = quantity;
        Status = OrderStatus.Pending;
    }

    public static Order Create(Guid bookId, int quantity)
    {
        var order = new Order(
            Guid.NewGuid(),
            BookId.Create(bookId),
            OrderQuantity.Create(quantity));

        return order;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be confirmed.");

        Status = OrderStatus.Confirmed;
        ConfirmedAtUtc = DateTime.UtcNow;

        MarkAsUpdated();
    }

    public void Fail(string reason)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be failed.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Failure reason is required.");

        Status = OrderStatus.Failed;
        FailureReason = reason;
        FailedAtUtc = DateTime.UtcNow;

        MarkAsUpdated();
    }

    public void Cancel(string reason)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be cancelled.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required.");

        Status = OrderStatus.Cancelled;
        FailureReason = reason;

        MarkAsUpdated();
    }
}
