using Mansari.Store.Catalog.Application.Books.DTOs;
using MediatR;


namespace Mansari.Store.Catalog.Application.Books.Command.UpdateBook;

public sealed record UpdateBookCommand(
    Guid Id,
    string Title,
    string Author,
    decimal Price
) : IRequest<BookDto?>;

