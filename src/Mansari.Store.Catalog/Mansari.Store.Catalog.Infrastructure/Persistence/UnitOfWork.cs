using Mansari.Store.Catalog.Domain.Interfaces;


namespace Mansari.Store.Catalog.Infrastructure.Persistence;

// دلیل اینکه از ریپازیتوری جداش کردم:
//commit is a transaction concern, and independed to repository! also: application shouldn't couple to dbContext

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CatalogDbContext _dbContext;

    public UnitOfWork(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}

