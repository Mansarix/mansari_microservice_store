using Mansari.Store.Catalog.Application.Books.DTOs;
using Mansari.Store.Catalog.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Application.Books.Queries.GetAllBooks;

public sealed class GetAllBooksQueryHandler
    : IRequestHandler<GetAllBooksQuery, IReadOnlyList<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IReadOnlyList<BookDto>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        var books = await _bookRepository
            .GetAllAsync(cancellationToken);

        return books.Select(book => new BookDto
        {
            Id = book.Id,
            Title = book.Title.Value,
            Author = book.Author.Value,
            Price = book.Price.Amount,
            Currency = book.Price.Currency,
            Stock = book.Stock.Value
        }).ToList();
    }
}
