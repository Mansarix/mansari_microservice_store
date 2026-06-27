namespace Mansari.Store.Catalog.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; init; } = "rabbitmq";
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
}

