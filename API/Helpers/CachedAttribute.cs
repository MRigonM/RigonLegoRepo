using System.Text;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class CachedAttribute : Attribute,IAsyncActionFilter  
{
    private readonly int _timeToLiveInSeconds;

    public CachedAttribute(int timeToLiveInSeconds)
    {
        _timeToLiveInSeconds = timeToLiveInSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
        var cacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);

        if (!string.IsNullOrEmpty(cacheResponse))
        {
            var contentResult = new ContentResult
            {
                Content = cacheResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            context.Result = contentResult;
            
            return;
        }

        var excutedContext = await next();

        if (excutedContext.Result is OkObjectResult okObjectResult)
        {
            await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
                TimeSpan.FromSeconds(_timeToLiveInSeconds));
        }
    }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();

        keyBuilder.Append($"{request.Path}");

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
    
}