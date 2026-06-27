namespace Mansari.Store.Ordering.Application.DTOs;

public sealed record CreateOrderResponse(Guid OrderId, int Quantity, bool Success, string? Error);

