using Mansari.Store.Catalog.Application.Interfaces.Messaging;
using Mansari.Store.Catalog.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Mansari.Store.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventBus(IOptions<RabbitMqOptions> options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.UserName,
            Password = options.Value.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        var eventName = typeof(T).Name;
        var payload = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(payload);

        _channel.ExchangeDeclare("catalog.events", ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(eventName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(eventName, "catalog.events", eventName);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(
            exchange: "catalog.events",
            routingKey: eventName,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
