using Mansari.Store.Catalog.Application.Books.DTOs;
using Mansari.Store.Catalog.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Mansari.Store.Infrastructure.Caching;

public sealed class RedisBookCacheService : IBookCacheService
{
    private const string CacheKeyPrefix = "book:";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    //چرا دیستریبیوت کش؟
    //چون Application فقط IBookCacheService را می‌بیند و Infrastructure هم به جای کار مستقیم با Redis low-level، از abstraction استاندارد استفاده می‌کند.
    private readonly IDistributedCache _distributedCache;

    public RedisBookCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<BookDto?> GetAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var cacheKey = BuildKey(bookId);
        var json = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<BookDto>(json, JsonOptions);
    }

    public async Task SetAsync(
        BookDto book,
        TimeSpan expiration,
        CancellationToken cancellationToken)
    {
        var cacheKey = BuildKey(book.Id);
        var json = JsonSerializer.Serialize(book, JsonOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        await _distributedCache.SetStringAsync(cacheKey, json, options, cancellationToken);
    }

    public Task RemoveAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var cacheKey = BuildKey(bookId);
        return _distributedCache.RemoveAsync(cacheKey, cancellationToken);
    }

    private static string BuildKey(Guid bookId) => $"{CacheKeyPrefix}{bookId}";
}
