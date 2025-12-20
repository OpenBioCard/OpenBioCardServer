using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenBioCardServer.Configuration;
using OpenBioCardServer.Interfaces;

namespace OpenBioCardServer.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache? _distributedCache;
    private readonly CacheSettings _settings;
    private readonly ILogger<CacheService> _logger;

    // 默认过期时间
    private readonly TimeSpan _defaultAbsoluteExpiration;
    private readonly TimeSpan _defaultSlidingExpiration;

    public CacheService(
        IMemoryCache memoryCache,
        IOptions<CacheSettings> settings,
        ILogger<CacheService> logger,
        IServiceProvider serviceProvider)
    {
        _memoryCache = memoryCache;
        _settings = settings.Value;
        _logger = logger;

        _defaultAbsoluteExpiration = TimeSpan.FromMinutes(_settings.ExpirationMinutes);
        _defaultSlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpirationMinutes);

        // 仅在启用 Redis 时解析 IDistributedCache
        if (_settings.UseRedis)
        {
            _distributedCache = serviceProvider.GetService<IDistributedCache>();
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (!_settings.Enabled) return default;

        try
        {
            if (_settings.UseRedis && _distributedCache != null)
            {
                var jsonStr = await _distributedCache.GetStringAsync(key);
                return string.IsNullOrEmpty(jsonStr) ? default : JsonSerializer.Deserialize<T>(jsonStr);
            }

            _memoryCache.TryGetValue(key, out T? value);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache read failed for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        if (!_settings.Enabled || value == null) return;

        var absExp = absoluteExpiration ?? _defaultAbsoluteExpiration;
        var slideExp = slidingExpiration ?? _defaultSlidingExpiration;

        try
        {
            if (_settings.UseRedis && _distributedCache != null)
            {
                var jsonStr = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absExp
                };
                await _distributedCache.SetStringAsync(key, jsonStr, options);
            }
            else
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(absExp)
                    .SetSlidingExpiration(slideExp)
                    .SetSize(1); // 简单的 Size 设为 1，配合 Program.cs 的 SizeLimit

                _memoryCache.Set(key, value, options);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache write failed for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        if (!_settings.Enabled) return;

        try
        {
            if (_settings.UseRedis && _distributedCache != null)
            {
                await _distributedCache.RemoveAsync(key);
            }
            else
            {
                _memoryCache.Remove(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache remove failed for key: {Key}", key);
        }
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? absoluteExpiration = null)
    {
        if (!_settings.Enabled)
        {
            return await factory();
        }

        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var newValue = await factory();
        
        if (newValue != null)
        {
            await SetAsync(key, newValue, absoluteExpiration);
        }

        return newValue;
    }
}
