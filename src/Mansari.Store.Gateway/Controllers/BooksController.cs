//using Microsoft.AspNetCore.Mvc;

//namespace Mansari.Store.Gateway.Controllers;

//[ApiController]
//[Route("api/books")]
//public class BooksController : ControllerBase
//{
//    private readonly CatalogService.CatalogServiceClient _catalog;

//    public BooksController(CatalogService.CatalogServiceClient catalog)
//    {
//        _catalog = catalog;
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> Get(Guid id)
//    {
//        var result = await _catalog.GetBookByIdAsync(new GetBookRequest
//        {
//            Id = id.ToString()
//        });

//        return Ok(result);
//    }
//}
