using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Demo.Api.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }

    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _serializerOptions;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json, _serializerOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromMinutes(30)
            };

            var json = JsonSerializer.Serialize(value, _serializerOptions);
            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var value = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
    }


}
