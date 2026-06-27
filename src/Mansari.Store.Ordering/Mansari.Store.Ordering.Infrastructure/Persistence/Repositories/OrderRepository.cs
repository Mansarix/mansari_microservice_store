using Mansari.Store.Ordering.Domain.Orders.Entities;
using Mansari.Store.Ordering.Domain.Orders.Interfaces;
using Mansari.Store.Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _dbContext;

    public OrderRepository(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Order Order,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(Order, cancellationToken);
    }

    public void Update(Order Order)
    {
        _dbContext.Orders.Update(Order);
    }

    public void Remove(Order Order)
    {
        _dbContext.Orders.Remove(Order);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}
