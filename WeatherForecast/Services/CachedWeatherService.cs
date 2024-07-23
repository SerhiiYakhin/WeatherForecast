using Microsoft.Extensions.Caching.Memory;
using WeatherForecast.Models;

namespace WeatherForecast.Services;

public class CachedWeatherService(IWeatherService weatherService, IMemoryCache cache) : IWeatherService
{
    public async Task<IEnumerable<Forecast>> GetForecastsAsync(DateTime date, string city, string country)
    {
        var cacheKey = $"{date:yyyyMMdd}_{city}_{country}";
        if (cache.TryGetValue(cacheKey, out IEnumerable<Forecast> forecasts)) return forecasts;

        forecasts = await weatherService.GetForecastsAsync(date, city, country);
        cache.Set(cacheKey, forecasts, TimeSpan.FromMinutes(30));
        return forecasts;
    }
}