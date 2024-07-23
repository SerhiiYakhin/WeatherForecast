namespace WeatherForecast.Models.WeatherApiResponses.WeatherApi;

public record WeatherApiResponse
{
    public Location location { get; set; }
    public Forecast forecast { get; set; }
}