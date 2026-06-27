namespace Mansari.Store.Gateway.Models;

public class OrderDetailsResponse
{
    public string OrderId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public BookDto Book { get; set; }
}

public class BookDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public double Price { get; set; }
}

