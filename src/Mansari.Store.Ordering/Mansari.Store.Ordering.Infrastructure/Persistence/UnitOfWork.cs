using Mansari.Store.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Mansari.Store.Ordering.Domain.Abstractions;

namespace Mansari.Store.Ordering.Infrastructure.Persistence;

// دلیل اینکه از ریپازیتوری جداش کردم:
//commit is a transaction concern, and independed to repository! also: application shouldn't couple to dbContext

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OrderingDbContext _dbContext;

    public UnitOfWork(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

