namespace Mansari.Store.Ordering.Infrastructure.Persistence.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; } = default!;

    public DateTime ReceivedOnUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }

    public int RetryCount { get; set; } = 0;

    public DateTime? NextRetryOnUtc { get; set; }
}

