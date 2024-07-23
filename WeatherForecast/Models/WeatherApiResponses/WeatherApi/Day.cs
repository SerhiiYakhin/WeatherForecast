namespace WeatherForecast.Models.WeatherApiResponses.WeatherApi;

public record Day
{
    public double avgtemp_c { get; set; }
    public double maxtemp_c { get; set; }
    public double mintemp_c { get; set; }
    public Condition condition { get; set; }
}