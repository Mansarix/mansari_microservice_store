using Mansari.Store.Catalog.Application.Interfaces.Caching;
using Mansari.Store.Catalog.Application.Interfaces.Messaging;
using Mansari.Store.Catalog.Domain.Interfaces;
using Mansari.Store.Catalog.Infrastructure.Messaging;
using Mansari.Store.Catalog.Infrastructure.Messaging.Consumers;
using Mansari.Store.Catalog.Infrastructure.Persistence;
using Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;
using Mansari.Store.Catalog.Infrastructure.Persistence.Repositories;
using Mansari.Store.Infrastructure.Caching;
using Mansari.Store.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Mansari.Store.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CatalogDatabase")));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =
                configuration.GetConnectionString("Redis");

            options.InstanceName = "catalog:";
        });

        services.AddScoped<IBookCacheService, RedisBookCacheService>();

        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IConnection>(sp =>
        {
            var rabbitOptions = configuration
                .GetSection(RabbitMqOptions.SectionName)
                .Get<RabbitMqOptions>()!;

            var factory = new ConnectionFactory
            {
                HostName = rabbitOptions.HostName,
                UserName = rabbitOptions.UserName,
                Password = rabbitOptions.Password,
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        });

        services.AddSingleton<IEventBus, RabbitMqEventBus>();

        services.AddScoped<IOutboxService, OutboxService>();

        services.AddHostedService<OutboxProcessor>();

        services.AddHostedService<OrderCreatedEventConsumer>();

        return services;
    }
}
