using Mansari.Store.Catalog.Domain.Entities;

namespace Mansari.Store.Catalog.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Book>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Book book,
        CancellationToken cancellationToken = default);

    void Update(Book book);

    void Remove(Book book);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

