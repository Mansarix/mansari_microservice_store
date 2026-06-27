using Mansari.Store.Catalog.Application.Interfaces.Caching;
using Mansari.Store.Catalog.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Application.Books.Commands.DeleteBook;

public sealed class DeleteBookCommandHandler
    : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _cache;

    public DeleteBookCommandHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork,
        IBookCacheService cache)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<bool> Handle(
        DeleteBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (book is null)
            return false;

        // cache invalidation
        _bookRepository.Remove(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync(book.Id, cancellationToken);

        return true;
    }
}
