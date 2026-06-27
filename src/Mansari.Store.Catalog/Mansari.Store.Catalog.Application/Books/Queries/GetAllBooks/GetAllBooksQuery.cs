using Mansari.Store.Catalog.Application.Books.DTOs;
using MediatR;

namespace Mansari.Store.Application.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery()
    : IRequest<IReadOnlyList<BookDto>>;
