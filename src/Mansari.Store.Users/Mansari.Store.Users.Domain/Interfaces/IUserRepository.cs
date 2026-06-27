using Mansari.Store.Users.Domain.Entities;
using Mansari.Store.Users.Domain.Enums;

namespace Mansari.Store.Users.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<User?> GetByNationalCodeAsync(string nationalCode, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<User?> GetByMobileAsync(string mobileNumber, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNationalCodeAsync(string nationalCode, Guid? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMobileAsync(string mobileNumber, Guid? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, Guid? excludingId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, Guid? excludingId = null, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        UserStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Remove(User user);
}
