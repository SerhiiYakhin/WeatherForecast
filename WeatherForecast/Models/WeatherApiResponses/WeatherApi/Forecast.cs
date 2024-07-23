namespace WeatherForecast.Models.WeatherApiResponses.WeatherApi;

public record Forecast
{
    public List<ForecastDay> forecastday { get; set; }
}