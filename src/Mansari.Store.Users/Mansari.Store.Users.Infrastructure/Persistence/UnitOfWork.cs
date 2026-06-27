using Mansari.Store.Users.Domain.Interfaces;

namespace Mansari.Store.Users.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly UsersDbContext _dbContext;

    public UnitOfWork(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
