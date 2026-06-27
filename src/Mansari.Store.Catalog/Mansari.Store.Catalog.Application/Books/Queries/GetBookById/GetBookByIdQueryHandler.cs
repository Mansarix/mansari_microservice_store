using Mansari.Store.Catalog.Application.Books.DTOs;
using Mansari.Store.Catalog.Application.Interfaces.Caching;
using Mansari.Store.Catalog.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Catalog.Application.Books.Queries.GetBookById;

public sealed class GetBookByIdQueryHandler
    : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookCacheService _cacheService;

    public GetBookByIdQueryHandler(
        IBookRepository bookRepository,
        IBookCacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task<BookDto?> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cachedBook = await _cacheService
            .GetAsync(request.BookId, cancellationToken);

        if (cachedBook is not null)
            return cachedBook;

        var book = await _bookRepository
            .GetByIdAsync(request.BookId, cancellationToken);

        if (book is null)
            return null;

        var dto = new BookDto
        {
            Id = book.Id,
            Title = book.Title.Value,
            Author = book.Author.Value,
            Price = book.Price.Amount,
            Stock = book.Stock.Value
        };

        await _cacheService.SetAsync(
            dto,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return dto;
    }
}

