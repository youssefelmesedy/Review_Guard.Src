namespace Review_Guard.Application.Abstractions.Caching;

public interface ICacheService
{
    /// <summary>
    /// Retrieves a value from the cache using the specified key.
    /// </summary>
    /// <typeparam name="T">The expected type of the cached value.</typeparam>
    /// <param name="key">The unique cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The cached value if found; otherwise <see langword="null"/>.
    /// </returns>
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a value in the cache with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The unique cache key.</param>
    /// <param name="value">The value to be stored.</param>
    /// <param name="expiry">
    /// Optional expiration time. If not provided, a default expiration will be used.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a specific item from the cache using its key.
    /// </summary>
    /// <param name="key">The cache key to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all cached entries whose keys start with the specified prefix.
    /// Useful for invalidating related cached data in bulk (e.g., lists or grouped data).
    /// </summary>
    /// <param name="prefix">The prefix used to match cache keys.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveByPrefixAsync(
        string prefix,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a value from the cache if it exists; otherwise, executes the provided factory,
    /// stores the result in the cache, and returns it.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">
    /// A function that generates the value if it is not found in the cache.
    /// </param>
    /// <param name="expiry">
    /// Optional expiration time for the cached value.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly generated value.</returns>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);
}