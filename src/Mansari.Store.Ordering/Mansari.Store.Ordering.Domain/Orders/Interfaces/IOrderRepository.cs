using Mansari.Store.Ordering.Domain.Orders.Entities;

namespace Mansari.Store.Ordering.Domain.Orders.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);

    void Update(Order order);

    void Remove(Order order);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

