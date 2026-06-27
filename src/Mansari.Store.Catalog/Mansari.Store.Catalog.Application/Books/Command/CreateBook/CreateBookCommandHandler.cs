using Mansari.Store.Catalog.Application.Books.DTOs;
using Mansari.Store.Catalog.Domain.Interfaces;
using Mansari.Store.Catalog.Domain.Entities;
using MediatR;

namespace Mansari.Store.Catalog.Application.Books.Command.CreateBook;

public sealed class CreateBookCommandHandler
    : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = Mansari.Store.Catalog.Domain.Entities.Book.Create(
            request.Title,
            request.Author,
            request.Stock,
            request.Price);

        await _bookRepository.AddAsync(book, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title.Value,
            Author = book.Author.Value,
            Price = book.Price.Amount,
            Stock = book.Stock.Value
        };
    }
}

