
//using Grpc.Core;
//using Mansari.Store.Catalog.Application.Books.Queries.GetBookById;
//using Mansari.Store.Catalog.Grpc;
//using MediatR;
//using Microsoft.AspNetCore.Http;

//namespace Mansari.Store.Catalog.Api.Grpc;

//public class CatalogGrpcService : CatalogService.CatalogServiceBase
//{
//    private readonly IMediator _mediator;

//    public CatalogGrpcService(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    public override async Task<BookResponse> GetBookById(
//        GetBookRequest request,
//        ServerCallContext context)
//    {
//        var book = await _mediator.Send(new GetBookByIdQuery(Guid.Parse(request.Id)));

//        if (book == null)
//            throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));

//        return new BookResponse
//        {
//            Id = book.Id.ToString(),
//            Title = book.Title,
//            Author = book.Author,
//            Price = (double)book.Price,
//            Stock = book.Stock
//        };
//    }
//}
