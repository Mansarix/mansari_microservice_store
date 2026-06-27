using System.Text;
using System.Text.Json;
using Mansari.Store.Contracts.Catalog;
using Mansari.Store.Ordering.Infrastructure.Persistence;
using Mansari.Store.Ordering.Infrastructure.Persistence.Inbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mansari.Store.Ordering.Infrastructure.Messaging.Consumers;

public sealed class StockReservedEventConsumer : BackgroundService
{
    private const string ExchangeName = "store.events";
    private const string QueueName = "order.stock-reserved";
    private const string RoutingKey = "stock.reserved";

    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly ILogger<StockReservedEventConsumer> _logger;

    private IModel? _channel;

    public StockReservedEventConsumer(
        IServiceProvider serviceProvider,
        IConnection connection,
        ILogger<StockReservedEventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _connection = connection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey);

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, eventArgs) =>
        {
            await HandleMessageAsync(_channel, eventArgs, stoppingToken);
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "StockReservedEventConsumer started. Queue: {QueueName}, RoutingKey: {RoutingKey}",
            QueueName,
            RoutingKey);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("StockReservedEventConsumer is stopping.");
        }
    }

    private async Task HandleMessageAsync(
        IModel channel,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        var messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {
            var integrationEvent = JsonSerializer.Deserialize<StockReservedEvent>(
                messageBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (integrationEvent is null)
            {
                _logger.LogWarning(
                    "Received invalid StockReservedEvent message. DeliveryTag: {DeliveryTag}",
                    eventArgs.DeliveryTag);

                channel.BasicReject(
                    deliveryTag: eventArgs.DeliveryTag,
                    requeue: false);

                return;
            }

            using var scope = _serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var alreadyProcessed = await dbContext.InboxMessages
                .AnyAsync(
                    x => x.Id == integrationEvent.Id && x.ProcessedOnUtc != null,
                    cancellationToken);

            if (alreadyProcessed)
            {
                await transaction.CommitAsync(cancellationToken);

                channel.BasicAck(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false);

                _logger.LogInformation(
                    "StockReservedEvent already processed. EventId: {EventId}",
                    integrationEvent.Id);

                return;
            }

            var inboxMessage = await dbContext.InboxMessages
                .FirstOrDefaultAsync(
                    x => x.Id == integrationEvent.Id,
                    cancellationToken);

            if (inboxMessage is null)
            {
                inboxMessage = new InboxMessage
                {
                    Id = integrationEvent.Id,
                    Type = nameof(StockReservedEvent),
                    ProcessedOnUtc = null
                };

                dbContext.InboxMessages.Add(inboxMessage);
            }

            var order = await dbContext.Orders
                .FirstOrDefaultAsync(
                    x => x.Id == integrationEvent.OrderId,
                    cancellationToken);

            if (order is null)
            {
                _logger.LogWarning(
                    "Order not found for StockReservedEvent. OrderId: {OrderId}, EventId: {EventId}",
                    integrationEvent.OrderId,
                    integrationEvent.Id);

                throw new InvalidOperationException(
                    $"Order with id '{integrationEvent.OrderId}' was not found.");
            }

            order.Confirm();

            inboxMessage.ProcessedOnUtc = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            channel.BasicAck(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false);

            _logger.LogInformation(
                "StockReservedEvent processed successfully. EventId: {EventId}, OrderId: {OrderId}",
                integrationEvent.Id,
                integrationEvent.OrderId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "StockReservedEvent processing was cancelled. DeliveryTag: {DeliveryTag}",
                eventArgs.DeliveryTag);

            channel.BasicNack(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "Failed to deserialize StockReservedEvent. Message: {Message}",
                messageBody);

            channel.BasicReject(
                deliveryTag: eventArgs.DeliveryTag,
                requeue: false);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to process StockReservedEvent. DeliveryTag: {DeliveryTag}",
                eventArgs.DeliveryTag);

            channel.BasicNack(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_channel is not null && _channel.IsOpen)
            {
                _channel.Close();
            }

            _channel?.Dispose();
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Error while closing RabbitMQ channel.");
        }

        return base.StopAsync(cancellationToken);
    }
}
