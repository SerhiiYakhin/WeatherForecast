using System.Net.Http;
using WeatherForecast.Models;
using WeatherForecast.Models.WeatherApiResponses.VisualCrossing;
using WeatherForecast.Models.WeatherApiResponses.WeatherApi;
using WeatherForecast.Models.WeatherApiResponses.WeatherBit;
using Forecast = WeatherForecast.Models.Forecast;

namespace WeatherForecast.Services;

public class WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherService> logger) : IWeatherService
{
    public async Task<IEnumerable<Forecast>> GetForecastsAsync(DateTime date, string city, string country)
    {
        var forecasts = new List<Forecast>();

        var weatherApiForecast = await GetWeatherFromWeatherApiAsync(date, city, country);
        if (weatherApiForecast != null) forecasts.Add(weatherApiForecast);

        var weatherBitForecast = await GetWeatherFromWeatherBitAsync(date, city, country);
        if (weatherBitForecast != null) forecasts.Add(weatherBitForecast);

        var visualCrossingForecast = await GetWeatherFromVisualCrossingAsync(date, city, country);
        if (visualCrossingForecast != null) forecasts.Add(visualCrossingForecast);

        return forecasts;
    }

    private async Task<Forecast?> GetWeatherFromWeatherApiAsync(DateTime date, string city, string country)
    {
        try
        {
            var apiKey = configuration["APIKeys:WeatherApiKey"];
            var client = httpClientFactory.CreateClient("WeatherApi");
            var response = await client.GetAsync($"https://api.weatherapi.com/v1/history.json?key={apiKey}&q={city}&dt={date:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Weather API returned non-success status code: {response.StatusCode}");
                return null;
            }

            var weatherApiResponse = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
            if (weatherApiResponse?.forecast?.forecastday?.Count > 0)
            {
                var forecastDay = weatherApiResponse.forecast.forecastday[0];
                return new Forecast(
                    Date: DateTime.Parse(forecastDay.date),
                    City: weatherApiResponse.location.name,
                    Country: weatherApiResponse.location.country,
                    Source: "WeatherAPI",
                    Description: forecastDay.day.condition.text,
                    Temperature: new Temperature(
                        Avg: forecastDay.day.avgtemp_c,
                        Min: forecastDay.day.mintemp_c,
                        Max: forecastDay.day.maxtemp_c
                    )
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching weather data from Weather API");
        }

        return null;
    }

    private async Task<Forecast?> GetWeatherFromWeatherBitAsync(DateTime date, string city, string country)
    {
        try
        {
            var apiKey = configuration["APIKeys:WeatherBit"];
            var client = httpClientFactory.CreateClient("WeatherBit");
            var response = await client.GetAsync($"https://api.weatherbit.io/v2.0/history/daily?city={city}&start_date={date:yyyy-MM-dd}&end_date={date.AddDays(1):yyyy-MM-dd}&key={apiKey}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"WeatherBit API returned non-success status code: {response.StatusCode}");
                return null;
            }

            var weatherbitResponse = await response.Content.ReadFromJsonAsync<WeatherBitResponse>();
            if (weatherbitResponse?.data?.Count > 0)
            {
                var weatherData = weatherbitResponse.data[0];
                return new Forecast(
                    Date: DateTime.Parse(weatherData.datetime),
                    City: city,
                    Country: country,
                    Source: "WeatherBit",
                    Description: null,
                    Temperature: new Temperature(
                        Avg: weatherData.temp,
                        Min: weatherData.min_temp,
                        Max: weatherData.max_temp
                    )
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching weather data from WeatherBit API");
        }

        return null;
    }

    private async Task<Forecast?> GetWeatherFromVisualCrossingAsync(DateTime date, string city, string country)
    {
        try
        {
            var apiKey = configuration["APIKeys:VisualCrossing"];
            var client = httpClientFactory.CreateClient("VisualCrossing");
            var response = await client.GetAsync($"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city}/{date:yyyy-MM-dd}?unitGroup=metric&key={apiKey}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Visual Crossing API returned non-success status code: {response.StatusCode}");
                return null;
            }

            var visualCrossingResponse = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>();
            if (visualCrossingResponse?.days?.Count > 0)
            {
                var weatherData = visualCrossingResponse.days[0];
                return new Forecast(
                    Date: DateTime.Parse(weatherData.datetime),
                    City: city,
                    Country: country,
                    Source: "VisualCrossing",
                    Description: weatherData.conditions,
                    Temperature: new Temperature(
                        Avg: weatherData.temp,
                        Min: weatherData.tempmin,
                        Max: weatherData.tempmax
                    )
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching weather data from Visual Crossing API");
        }

        return null;
    }
}