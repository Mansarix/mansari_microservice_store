
namespace Mansari.Store.Catalog.Application.Books.DTOs;

public sealed class BookDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string Author { get; init; } = default!;
    public decimal Price { get; init; }
    public string Currency { get; init; } = default!;
    public int Stock { get; init; }
}

