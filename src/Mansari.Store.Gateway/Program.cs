var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddGrpcClient<CatalogService.CatalogServiceClient>(o =>
//{
//    o.Address = new Uri("http://catalog-service:5001");
//});

//builder.Services.AddGrpcClient<OrderingService.OrderingServiceClient>(o =>
//{
//    o.Address = new Uri("http://ordering-service:5002");
//});

//builder.Services.AddScoped<OrderDetailsService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
