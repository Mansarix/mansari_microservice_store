using System.ComponentModel.DataAnnotations;

namespace Mansari.Store.Ordering.Api.Contracts;

public sealed record CreateOrderRequest
{
    [Required]
    public Guid BookId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; init; }
}