namespace Core.Interfaces;

public interface IResponseCacheService
{
    Task CacheResponseAsync(string cacheKey, object reponse, TimeSpan timeToLive);
    Task<string> GetCachedResponseAsync(string cacheKey);
}