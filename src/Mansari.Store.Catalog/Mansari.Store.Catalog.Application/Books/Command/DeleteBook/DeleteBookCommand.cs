using MediatR;

namespace Mansari.Store.Application.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(Guid Id)
    : IRequest<bool>;
