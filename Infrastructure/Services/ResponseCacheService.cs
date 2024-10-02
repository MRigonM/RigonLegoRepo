using System.Text.Json;
using Core.Interfaces;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.Services;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDatabase _database;
    public ResponseCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task CacheResponseAsync(string cacheKey, object reponse, TimeSpan timeToLive)
    {
        if (reponse == null)
        {
            return;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var serialisedResponse = JsonSerializer.Serialize(reponse, options);

        await _database.StringSetAsync(cacheKey, serialisedResponse, timeToLive);
    }

    public async Task<string> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);

        if (cachedResponse.IsNullOrEmpty)
        {
            return null;
        }

        return cachedResponse;
    }
}