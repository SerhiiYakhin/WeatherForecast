namespace WeatherForecast.Models.WeatherApiResponses.WeatherApi;

public record ForecastDay
{
    public string date { get; set; }
    public Day day { get; set; }
}