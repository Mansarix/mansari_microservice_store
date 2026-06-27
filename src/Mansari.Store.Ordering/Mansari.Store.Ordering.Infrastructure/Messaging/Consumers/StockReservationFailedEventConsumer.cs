using Mansari.Store.Contracts.Catalog;
using Mansari.Store.Ordering.Infrastructure.Persistence;
using Mansari.Store.Ordering.Infrastructure.Persistence.Inbox;
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

namespace Mansari.Store.Ordering.Infrastructure.Messaging.Consumers;

public sealed class StockReservationFailedEventConsumer : BackgroundService
{
    private const string ExchangeName = "store.events";
    private const string QueueName = "order.stock-reservation-failed";
    private const string RoutingKey = "stock.reservation.failed";
    private const int MaxRetryCount = 5;

    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly ILogger<StockReservationFailedEventConsumer> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public StockReservationFailedEventConsumer(
        IServiceProvider serviceProvider,
        IConnection connection,
        ILogger<StockReservationFailedEventConsumer> logger)
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
                        "Retry attempt {Attempt} will be made after {Delay} for {Consumer}.",
                        attempt,
                        delay,
                        nameof(StockReservationFailedEventConsumer));
                });
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        channel.QueueBind(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey);

        channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += async (_, eventArgs) =>
        {
            await HandleMessageAsync(channel, eventArgs, stoppingToken);
        };

        channel.BasicConsume(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(
        IModel channel,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        StockFailedEvent? integrationEvent;

        try
        {
            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            integrationEvent = JsonSerializer.Deserialize<StockFailedEvent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (integrationEvent is null)
            {
                _logger.LogWarning("Invalid message received in {Consumer}.", nameof(StockReservationFailedEventConsumer));
                channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize message in {Consumer}.", nameof(StockReservationFailedEventConsumer));
            channel.BasicReject(eventArgs.DeliveryTag, requeue: false);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        var inboxMessage = await dbContext.InboxMessages
            .FirstOrDefaultAsync(x => x.Id == integrationEvent.Id, cancellationToken);

        if (inboxMessage is not null && inboxMessage.ProcessedOnUtc is not null)
        {
            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            return;
        }

        if (inboxMessage is null)
        {
            inboxMessage = new InboxMessage
            {
                Id = integrationEvent.Id,
                Type = nameof(StockFailedEvent),
                ReceivedOnUtc = DateTime.UtcNow,
                RetryCount = 0
            };

            dbContext.InboxMessages.Add(inboxMessage);
        }

        if (inboxMessage.NextRetryOnUtc is not null && inboxMessage.NextRetryOnUtc > DateTime.UtcNow)
        {
            _logger.LogInformation(
                "Message {MessageId} is scheduled for retry at {NextRetryOnUtc}.",
                inboxMessage.Id,
                inboxMessage.NextRetryOnUtc);

            channel.BasicNack(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);

            return;
        }

        try
        {
            await _retryPolicy.ExecuteAsync(async ct =>
            {
                var order = await dbContext.Orders
                    .FirstOrDefaultAsync(x => x.Id == integrationEvent.OrderId, ct);

                if (order is null)
                {
                    throw new InvalidOperationException($"Order '{integrationEvent.OrderId}' not found.");
                }

                order.Fail(integrationEvent.Reason);

                inboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                inboxMessage.Error = null;
                inboxMessage.NextRetryOnUtc = null;

                await dbContext.SaveChangesAsync(ct);
            }, cancellationToken);

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            inboxMessage.RetryCount += 1;
            inboxMessage.Error = ex.Message;

            if (inboxMessage.RetryCount >= MaxRetryCount)
            {
                inboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                inboxMessage.NextRetryOnUtc = null;

                _logger.LogError(
                    ex,
                    "Message {MessageId} failed permanently after {RetryCount} retries.",
                    inboxMessage.Id,
                    inboxMessage.RetryCount);

                await dbContext.SaveChangesAsync(cancellationToken);

                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                return;
            }

            inboxMessage.NextRetryOnUtc = DateTime.UtcNow.AddMinutes(Math.Pow(2, inboxMessage.RetryCount));

            _logger.LogError(
                ex,
                "Failed to process message {MessageId}. RetryCount: {RetryCount}, NextRetryOnUtc: {NextRetryOnUtc}",
                inboxMessage.Id,
                inboxMessage.RetryCount,
                inboxMessage.NextRetryOnUtc);

            await dbContext.SaveChangesAsync(cancellationToken);

            channel.BasicNack(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                requeue: true);
        }
    }
}
