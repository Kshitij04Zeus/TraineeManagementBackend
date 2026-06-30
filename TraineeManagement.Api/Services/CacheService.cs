using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTO;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace TraineeManagement.Api.Services;
public class CacheService:ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly RedisSettings _settings;
    private static readonly JsonSerializerOptions _jsonOptions=
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive=true
    };
    public CacheService(IDistributedCache cache, ILogger<CacheService> logger,IOptions<RedisSettings> options) 
    {
        _cache = cache;
        _logger = logger;
        _settings=options.Value;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedValue=await _cache.GetStringAsync(key);
            if(string.IsNullOrWhiteSpace(cachedValue))
            {
                _logger.LogInformation("Cache MISS for key {CacheKey}",key);
                return default;
            }
            _logger.LogInformation("Cache HIT for key {CacheKey}",key);
            return JsonSerializer.Deserialize<T>(cachedValue,_jsonOptions);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Redis has failed. Retrieving from database");
            return default;
        }
    }

    public async Task SetAsync<T>(string key,T value,TimeSpan? absoluteExpiration=null)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be blank");
            var serializedValue=JsonSerializer.Serialize(value);
            var options=new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow=absoluteExpiration ?? TimeSpan.FromMinutes(_settings.DefaultTtlMinutes)
            };
            await _cache.SetStringAsync(key,serializedValue,options);
            _logger.LogInformation("Cache SET for key {CacheKey}",key);            
        }
        catch (Exception e)
        {
            _logger.LogWarning("Redis connection has failed. Unable to set key {key}",key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _logger.LogInformation("Cache REMOVE for key {CacheKey}",key);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Redis has failed. Unable to delete key {key}",key);
        }
    }
}