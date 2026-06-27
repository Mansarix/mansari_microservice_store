using Mansari.Store.Ordering.Infrastructure.Messaging;
using Mansari.Store.Ordering.Infrastructure.Messaging.Consumers;
using Mansari.Store.Ordering.Infrastructure.Persistence;
using Mansari.Store.Ordering.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Mansari.Store.Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddRabbitMq(services, configuration);
        AddMessaging(services);

        return services;
    }

    private static void AddDatabase(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("OrderingDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'OrderingDatabase' was not found.");

        services.AddDbContext<OrderingDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    private static void AddRabbitMq(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
        {
            configuration.GetSection(RabbitMqOptions.SectionName).Bind(options);
        });



        services.AddSingleton<IConnection>(sp =>
        {
            var options =
                sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        });
    }

    private static void AddMessaging(IServiceCollection services)
    {
        services.AddHostedService<StockReservedEventConsumer>();

        services.AddHostedService<StockReservationFailedEventConsumer>();

        services.AddHostedService<OutboxProcessor>();
    }
}
