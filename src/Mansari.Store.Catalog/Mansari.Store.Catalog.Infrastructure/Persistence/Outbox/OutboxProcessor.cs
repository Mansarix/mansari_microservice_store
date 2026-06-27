using Mansari.Store.Contracts.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;

namespace Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private const string ExchangeName = "store.events";

    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        IConnection connection,
        ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _connection = connection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Catalog OutboxProcessor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Catalog OutboxProcessor is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error occurred while processing catalog outbox messages.");

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var messages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (messages.Count == 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            return;
        }

        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        foreach (var message in messages)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.Content);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = message.Id.ToString();
                properties.Type = message.Type;
                properties.Timestamp = new AmqpTimestamp(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                var routingKey = GetRoutingKey(message.Type);

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;

                _logger.LogInformation(
                    "Catalog outbox message {MessageId} published successfully with routing key {RoutingKey}.",
                    message.Id,
                    routingKey);
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;

                _logger.LogError(
                    ex,
                    "Failed to publish catalog outbox message {MessageId}.",
                    message.Id);
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }

    private static string GetRoutingKey(string type)
    {
        if (type.Contains(nameof(StockReservedEvent), StringComparison.OrdinalIgnoreCase))
        {
            return "stock.reserved";
        }

        if (type.Contains(nameof(StockFailedEvent), StringComparison.OrdinalIgnoreCase))
        {
            return "stock.reservation.failed";
        }

        return "unknown.event";
    }
}
