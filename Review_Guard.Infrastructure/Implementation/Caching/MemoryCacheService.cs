namespace Review_Guard.Infrastructure.Implementation.Caching;

internal sealed class MemoryCacheService : ICacheService
{
    private readonly IDistributedCache _distributed;
    private readonly IMemoryCache _memory;
    private readonly ILogger<MemoryCacheService> _logger;

    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(10);

    private static readonly ConcurrentDictionary<string, byte> CacheKeys = new();

    public MemoryCacheService(
        IDistributedCache distributed,
        IMemoryCache memory,
        ILogger<MemoryCacheService> logger)
    {
        _distributed = distributed;
        _memory = memory;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var bytes = await _distributed.GetAsync(key, ct);

            if (bytes is not null)
            {
                _logger.LogInformation("Redis cache HIT for key {Key}", key);
                return JsonSerializer.Deserialize<T>(bytes);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GET failed for key {Key}", key);
        }

        var found = _memory.TryGetValue(key, out T? cached);

        if (found)
            _logger.LogInformation("Memory cache HIT for key {Key}", key);
        else
            _logger.LogInformation("Cache MISS for key {Key}", key);

        return cached;
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        var ttl = expiry ?? DefaultExpiry;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);

        _memory.Set(key, value, ttl);

        CacheKeys.TryAdd(key, 0);

        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            await _distributed.SetAsync(key, bytes, options, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SET failed for key {Key}", key);
        }

        _logger.LogInformation("Cache SET for key {Key}", key);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memory.Remove(key);
        CacheKeys.TryRemove(key, out _);

        try
        {
            await _distributed.RemoveAsync(key, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
        }

        _logger.LogInformation("Cache REMOVE for key {Key}", key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var keys = CacheKeys.Keys
            .Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keys)
        {
            _memory.Remove(key);
            CacheKeys.TryRemove(key, out _);

            try
            {
                await _distributed.RemoveAsync(key, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
            }
        }

        _logger.LogInformation(
            "Cache REMOVE BY PREFIX {Prefix}. Removed {Count} keys",
            prefix,
            keys.Count);
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        var cached = await GetAsync<T>(key, ct);

        if (cached is not null)
            return cached;

        var value = await factory();

        await SetAsync(key, value, expiry, ct);

        return value;
    }
}