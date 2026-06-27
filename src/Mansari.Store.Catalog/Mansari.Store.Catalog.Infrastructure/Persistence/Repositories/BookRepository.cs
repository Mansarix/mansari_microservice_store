using Mansari.Store.Catalog.Domain.Entities;
using Mansari.Store.Catalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly CatalogDbContext _dbContext;

    public BookRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Book book,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Books.AddAsync(book, cancellationToken);
    }

    public void Update(Book book)
    {
        _dbContext.Books.Update(book);
    }

    public void Remove(Book book)
    {
        _dbContext.Books.Remove(book);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Books
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}
