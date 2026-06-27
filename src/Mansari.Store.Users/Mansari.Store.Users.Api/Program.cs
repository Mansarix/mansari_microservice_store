using Mansari.Store.Users.Api.Interceptors;
using Mansari.Store.Users.Application;
using Mansari.Store.Users.Infrastructure;
using Mansari.Store.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionHandlingInterceptor>();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<UsersGrpcService>();
app.MapControllers();

app.MapGet("/", () => Results.Ok("Mansari Store Users gRPC service is running."));
app.MapGet("/health", async (UsersDbContext dbContext, CancellationToken cancellationToken) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
    return canConnect
        ? Results.Ok(new { status = "Healthy" })
        : Results.Problem("Database connection failed.");
});

app.Run();
