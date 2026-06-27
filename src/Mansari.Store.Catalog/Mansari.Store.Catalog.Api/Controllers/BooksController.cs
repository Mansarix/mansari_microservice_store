using Mansari.Store.Application.Books.Commands.DeleteBook;
using Mansari.Store.Application.Books.Queries.GetAllBooks;
using Mansari.Store.Catalog.Application.Books.Command.CreateBook;
using Mansari.Store.Catalog.Application.Books.Command.UpdateBook;
using Mansari.Store.Catalog.Application.Books.Queries.GetBookById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mansari.Store.API.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _mediator.Send(new GetBookByIdQuery(id));

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _mediator.Send(new GetAllBooksQuery());

        return Ok(books);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookCommand command)
    {
        var book = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = book.Id },
            book);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await _mediator.Send(command);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _mediator.Send(new DeleteBookCommand(id));

        if (!success)
            return NotFound();

        return NoContent();
    }
}
