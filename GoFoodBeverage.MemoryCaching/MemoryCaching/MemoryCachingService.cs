using Microsoft.Extensions.Caching.Memory;
using System;

namespace GoFoodBeverage.MemoryCaching
{
    public class MemoryCachingService : IMemoryCachingService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCachingService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetCache<T>(string key)
        {
            var value = _memoryCache.Get<T>(key);

            return value ?? default;
        }

        public void SetCache<T>(string key, T data)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
            _memoryCache.Set(key, data, cacheEntryOptions);
        }

        public void SetCache<T>(string key, T data, int slidingExpirationFromMinutes)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationFromMinutes) };
            _memoryCache.Set(key, data, cacheEntryOptions);
        }
    }
}
