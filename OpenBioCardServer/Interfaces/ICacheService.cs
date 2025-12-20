namespace OpenBioCardServer.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
    Task RemoveAsync(string key);
    
    /// <summary>
    /// 尝试获取缓存，如果不存在则执行 Factory 获取数据并写入缓存
    /// </summary>
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? absoluteExpiration = null);
}