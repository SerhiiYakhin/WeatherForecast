using WeatherForecast.Models;

namespace WeatherForecast.Services;

public interface IWeatherService
{
    Task<IEnumerable<Forecast>> GetForecastsAsync(DateTime date, string city, string country);
}