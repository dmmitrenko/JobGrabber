using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using WebScraperFunction.Domain.Models;
using WebScraperFunction.Infrastructure;
using WebScraperFunction.Infrastructure.Settings;

namespace WebScraperFunction.Application.Cache;
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;
    private readonly CacheSettings _cacheSettings;

    public RedisCacheService(
        IDatabase cache,
        IOptions<CacheSettings> cacheSettings)
    {
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<List<Vacancy>> GetCachedVacancies(string cacheKey)
    {
        var data = await _cache.StringGetAsync(cacheKey);
        if (data.IsNullOrEmpty)
            return new List<Vacancy>();

        return JsonConvert.DeserializeObject<List<Vacancy>>(data!)!;
    }

    public async Task SetCachedVacancies(string key, List<Vacancy> vacancies)
    {
        var vacanciesJson = JsonConvert.SerializeObject(vacancies);
        await _cache.StringSetAsync(key, vacanciesJson, _cacheSettings.Expiry);
    }

    public async Task<List<Vacancy>> GetNewVacancies(string cacheKey, DateTime lastCheckTime)
    {
        var vacancies = await GetCachedVacancies(cacheKey);
        return vacancies?.Where(v => v.PostedDate >= lastCheckTime).ToList();
    }
}
