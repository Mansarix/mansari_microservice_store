using Mansari.Store.Catalog.Application.Books.DTOs;
using MediatR;

namespace Mansari.Store.Catalog.Application.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery(Guid BookId)
    : IRequest<BookDto?>;
