using Mansari.Store.Catalog.Application.Books.DTOs;
using MediatR;

namespace Mansari.Store.Catalog.Application.Books.Command.CreateBook;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    int Stock,
    decimal Price
) : IRequest<BookDto>;

