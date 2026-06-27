using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using RabbitMQ.Client;
using System.Text;

namespace Mansari.Store.Ordering.Infrastructure.Persistence.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly AsyncPolicyWrap _policy;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        IConnection connection,
        ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _connection = connection;
        _logger = logger;

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry =>
                    TimeSpan.FromSeconds(Math.Pow(2, retry)));

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));

        _policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while processing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

        var now = DateTime.UtcNow;

        var messages = await dbContext.OutboxMessages
            .Where(x =>
                x.ProcessedOnUtc == null &&
                (x.NextRetryOnUtc == null || x.NextRetryOnUtc <= now))
            .OrderBy(x => x.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        using var channel = _connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: "store.events",
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
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                var routingKey = GetRoutingKey(message.Type);

                await _policy.ExecuteAsync(() =>
                {
                    channel.BasicPublish(
                        exchange: "store.events",
                        routingKey: routingKey,
                        mandatory: false,
                        basicProperties: properties,
                        body: body);

                    return Task.CompletedTask;
                });

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                message.NextRetryOnUtc = CalculateNextRetry(message.RetryCount);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static DateTime CalculateNextRetry(int retryCount)
    {
        var delaySeconds = retryCount switch
        {
            1 => 5,
            2 => 15,
            3 => 30,
            4 => 60,
            _ => 300
        };

        return DateTime.UtcNow.AddSeconds(delaySeconds);
    }

    private static string GetRoutingKey(string type)
    {
        if (type.Contains("OrderCreatedEvent"))
        {
            return "order.created";
        }

        if (type.Contains("StockReservedEvent"))
        {
            return "stock.reserved";
        }

        if (type.Contains("StockReservationFailedEvent"))
        {
            return "stock.reservation.failed";
        }

        return "unknown.event";
    }
}
