using Mansari.Store.Catalog.Application.Books.Command.UpdateBook;
using Mansari.Store.Catalog.Application.Books.DTOs;
using Mansari.Store.Catalog.Application.Interfaces.Caching;
using Mansari.Store.Catalog.Domain.Interfaces;
using MediatR;

namespace Mansari.Store.Application.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandHandler
    : IRequestHandler<UpdateBookCommand, BookDto?>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookCacheService _cache;

    public UpdateBookCommandHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork,
        IBookCacheService cache)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<BookDto?> Handle(
        UpdateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (book is null)
            return null;

        book.UpdateDetails(
            request.Title,
            request.Author,
            request.Price);

        _bookRepository.Update(book);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // cache invalidation
        await _cache.RemoveAsync(book.Id, cancellationToken);

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title.Value,
            Author = book.Author.Value,
            Price = book.Price.Amount,
            Currency = book.Price.Currency,
            Stock = book.Stock.Value
        };
    }
}
