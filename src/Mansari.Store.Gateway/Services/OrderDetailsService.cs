//public class OrderDetailsService
//{
//    private readonly OrderingService.OrderingServiceClient _ordering;
//    private readonly CatalogService.CatalogServiceClient _catalog;

//    public OrderDetailsService(
//        OrderingService.OrderingServiceClient ordering,
//        CatalogService.CatalogServiceClient catalog)
//    {
//        _ordering = ordering;
//        _catalog = catalog;
//    }

//    public async Task<OrderDetailsResponse> GetOrderDetails(Guid orderId)
//    {
//        var order = await _ordering.GetOrderByIdAsync(new GetOrderRequest
//        {
//            Id = orderId.ToString()
//        });

//        var book = await _catalog.GetBookByIdAsync(new GetBookRequest
//        {
//            Id = order.BookId
//        });

//        return new OrderDetailsResponse
//        {
//            OrderId = order.Id,
//            Quantity = order.Quantity,
//            Status = order.Status,
//            Book = new BookDto
//            {
//                Id = book.Id,
//                Title = book.Title,
//                Author = book.Author,
//                Price = book.Price
//            }
//        };
//    }
//}
