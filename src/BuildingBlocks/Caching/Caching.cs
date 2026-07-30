using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace EnterpriseSaaS.Caching;

public sealed record CacheEntryOptions(TimeSpan? AbsoluteExpiration = null, TimeSpan? SlidingExpiration = null);
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CacheEntryOptions options, CancellationToken cancellationToken = default);
}
public sealed class DistributedCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    { var bytes = await cache.GetAsync(key, cancellationToken); return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes); }
    public Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken cancellationToken = default) => cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(value), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = options.AbsoluteExpiration, SlidingExpiration = options.SlidingExpiration }, cancellationToken);
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) => cache.RemoveAsync(key, cancellationToken);
    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CacheEntryOptions options, CancellationToken cancellationToken = default)
    { var value = await GetAsync<T>(key, cancellationToken); if (value is not null) return value; value = await factory(cancellationToken); await SetAsync(key, value, options, cancellationToken); return value; }
}
public static class TenantCacheKey { public static string Create(Guid tenantId, string area, string key) => $"tenant:{tenantId:N}:{area}:{key}"; }
