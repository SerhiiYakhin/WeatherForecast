namespace WeatherForecast.Models.WeatherApiResponses.WeatherApi;

public record Location
{
    public string name { get; set; }
    public string country { get; set; }
    public string localtime { get; set; }
}