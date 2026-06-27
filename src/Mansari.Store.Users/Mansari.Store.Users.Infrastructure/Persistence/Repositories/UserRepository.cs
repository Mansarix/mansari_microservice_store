using Mansari.Store.Users.Domain.Entities;
using Mansari.Store.Users.Domain.Enums;
using Mansari.Store.Users.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Users.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UsersDbContext _dbContext;

    public UserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public Task<User?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted ? _dbContext.Users.IgnoreQueryFilters() : _dbContext.Users;
        return query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User?> GetByNationalCodeAsync(string nationalCode, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted ? _dbContext.Users.IgnoreQueryFilters() : _dbContext.Users;
        return query.FirstOrDefaultAsync(x => x.NationalCode.Value == Mansari.Store.Users.Domain.ValueObjects.NationalCode.Normalize(nationalCode), cancellationToken);
    }

    public Task<User?> GetByMobileAsync(string mobileNumber, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted ? _dbContext.Users.IgnoreQueryFilters() : _dbContext.Users;
        return query.FirstOrDefaultAsync(x => x.MobileNumber.Value == Mansari.Store.Users.Domain.ValueObjects.MobileNumber.Normalize(mobileNumber), cancellationToken);
    }

    public Task<User?> GetByUsernameAsync(string username, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted ? _dbContext.Users.IgnoreQueryFilters() : _dbContext.Users;
        return query.FirstOrDefaultAsync(x => x.Username.Value == Mansari.Store.Users.Domain.ValueObjects.Username.Normalize(username), cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = includeDeleted ? _dbContext.Users.IgnoreQueryFilters() : _dbContext.Users;
        return query.FirstOrDefaultAsync(x => x.Email != null && x.Email.Value == Mansari.Store.Users.Domain.ValueObjects.EmailAddress.Normalize(email), cancellationToken);
    }

    public Task<bool> ExistsByNationalCodeAsync(string nationalCode, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        return Query(excludingId).AnyAsync(x => x.NationalCode.Value == Mansari.Store.Users.Domain.ValueObjects.NationalCode.Normalize(nationalCode), cancellationToken);
    }

    public Task<bool> ExistsByMobileAsync(string mobileNumber, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        return Query(excludingId).AnyAsync(x => x.MobileNumber.Value == Mansari.Store.Users.Domain.ValueObjects.MobileNumber.Normalize(mobileNumber), cancellationToken);
    }

    public Task<bool> ExistsByUsernameAsync(string username, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        return Query(excludingId).AnyAsync(x => x.Username.Value == Mansari.Store.Users.Domain.ValueObjects.Username.Normalize(username), cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludingId = null, CancellationToken cancellationToken = default)
    {
        return Query(excludingId).AnyAsync(x => x.Email != null && x.Email.Value == Mansari.Store.Users.Domain.ValueObjects.EmailAddress.Normalize(email), cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        UserStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(x =>
                x.FirstName.Value.Contains(term) ||
                x.LastName.Value.Contains(term) ||
                x.NationalCode.Value.Contains(term) ||
                x.MobileNumber.Value.Contains(term) ||
                x.Username.Value.Contains(term) ||
                (x.Email != null && x.Email.Value.Contains(term)));
        }

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Remove(User user)
    {
        _dbContext.Users.Remove(user);
    }

    private IQueryable<User> Query(Guid? excludingId)
    {
        var query = _dbContext.Users.IgnoreQueryFilters().AsQueryable();

        if (excludingId.HasValue)
            query = query.Where(x => x.Id != excludingId.Value);

        return query;
    }
}
