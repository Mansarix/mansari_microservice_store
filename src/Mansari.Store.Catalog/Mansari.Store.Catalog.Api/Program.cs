using Mansari.Store.Catalog.Application.Books.Command.CreateBook;
using Mansari.Store.Catalog.Infrastructure;
using Mansari.Store.Catalog.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateBookCommand).Assembly);
});

//builder.Services.AddGrpc();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<CatalogDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapGrpcService<CatalogGrpcService>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();