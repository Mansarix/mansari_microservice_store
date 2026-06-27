using Mansari.Store.Catalog.Application.Books.DTOs;

namespace Mansari.Store.Catalog.Application.Interfaces.Caching;

public interface IBookCacheService
{
    Task<BookDto?> GetAsync(Guid bookId, CancellationToken cancellationToken);

    Task SetAsync(BookDto book, TimeSpan expiration, CancellationToken cancellationToken);

    Task RemoveAsync(Guid bookId, CancellationToken cancellationToken);
}
