using Mansari.Store.Catalog.Infrastructure.Persistence;
using Mansari.Store.Catalog.Infrastructure.Persistence.Inbox;
using Mansari.Store.Catalog.Infrastructure.Persistence.Outbox;
using Mansari.Store.Contracts.Catalog;
using Mansari.Store.Contracts.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Mansari.Store.Catalog.Infrastructure.Messaging.Consumers;

public sealed class OrderCreatedEventConsumer : BackgroundService
{
    private const string ExchangeName = "store.events";
    private const string QueueName = "catalog.order-created";
    private const string RoutingKey = "order.created";
    private const int MaxRetryCount = 5;

    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    private IModel? _channel;

    public OrderCreatedEventConsumer(
        IServiceProvider serviceProvider,
        IConnection connection,
        ILogger<OrderCreatedEventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _connection = connection;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<DbUpdateException>()
            .Or<TimeoutException>()
            .Or<InvalidOperationException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, delay, attempt, _) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry attempt {Attempt} will be made after {Delay} in {Consumer}.",
                        attempt,
                        delay,
                        nameof(OrderCreatedEventConsumer));
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderCreatedEventConsumer started.");

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

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OrderCreatedEventConsumer is stopping.");
        }
    }

    private async Task HandleMessageAsync(
        IModel channel,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        OrderCreatedEvent? integrationEvent;

        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            integrationEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (integrationEvent is null)
            {
                _logger.LogWarning("Received invalid OrderCreatedEvent message.");

                channel.BasicReject(
                    deliveryTag: eventArgs.DeliveryTag,
                    requeue: false);

                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize OrderCreatedEvent message.");

            channel.BasicReject(
                deliveryTag: eventArgs.DeliveryTag,
                requeue: false);

            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var alreadyProcessed = await dbContext.InboxMessages
            .AnyAsync(
                x => x.Id == integrationEvent.Id && x.ProcessedOnUtc != null,
                cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogInformation(
                "Message {MessageId} already processed. Skipping.",
                integrationEvent.Id);

            channel.BasicAck(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false);

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
                Type = nameof(OrderCreatedEvent),
                ReceivedOnUtc = DateTime.UtcNow,
                RetryCount = 0
            };

            dbContext.InboxMessages.Add(inboxMessage);
        }

        try
        {
            await _retryPolicy.ExecuteAsync(async ct =>
            {
                var book = await dbContext.Books
                    .FirstOrDefaultAsync(
                        x => x.Id == integrationEvent.BookId,
                        ct);

                if (book is null)
                {
                    var failedEvent = new StockFailedEvent(
                        integrationEvent.OrderId,
                        integrationEvent.BookId,
                        integrationEvent.Quantity,
                        "Book not found.",
                        integrationEvent.CorrelationId);

                    dbContext.OutboxMessages.Add(
                        OutboxMessageFactory.Create(failedEvent));

                    return;
                }

                if (book.Stock.Value < integrationEvent.Quantity)
                {
                    var failedEvent = new StockFailedEvent(
                        integrationEvent.OrderId,
                        integrationEvent.BookId,
                        integrationEvent.Quantity,
                        "Insufficient stock.",
                        integrationEvent.CorrelationId);

                    dbContext.OutboxMessages.Add(
                        OutboxMessageFactory.Create(failedEvent));

                    return;
                }

                book.DecreaseStock(integrationEvent.Quantity);

                var stockReservedEvent = new StockReservedEvent(
                    integrationEvent.OrderId,
                    integrationEvent.BookId,
                    integrationEvent.Quantity,
                    integrationEvent.CorrelationId);

                dbContext.OutboxMessages.Add(
                    OutboxMessageFactory.Create(stockReservedEvent));
            }, cancellationToken);

            inboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            inboxMessage.Error = null;
            inboxMessage.NextRetryOnUtc = null;

            await dbContext.SaveChangesAsync(cancellationToken);

            channel.BasicAck(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false);

            _logger.LogInformation(
                "OrderCreatedEvent {MessageId} processed successfully.",
                integrationEvent.Id);
        }
        catch (Exception ex)
        {
            inboxMessage.RetryCount += 1;
            inboxMessage.Error = ex.Message;

            if (inboxMessage.RetryCount >= MaxRetryCount)
            {
                inboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                inboxMessage.NextRetryOnUtc = null;

                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "OrderCreatedEvent {MessageId} failed permanently after {RetryCount} retries.",
                    integrationEvent.Id,
                    inboxMessage.RetryCount);

                channel.BasicAck(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false);

                return;
            }

            inboxMessage.NextRetryOnUtc = DateTime.UtcNow.AddMinutes(Math.Pow(2, inboxMessage.RetryCount));

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogError(
                ex,
                "Failed to process OrderCreatedEvent {MessageId}. RetryCount: {RetryCount}, NextRetryOnUtc: {NextRetryOnUtc}",
                integrationEvent.Id,
                inboxMessage.RetryCount,
                inboxMessage.NextRetryOnUtc);

            channel.BasicNack(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }

    public override void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
        }
        catch
        {
            // Ignore dispose errors.
        }

        base.Dispose();
    }
}
