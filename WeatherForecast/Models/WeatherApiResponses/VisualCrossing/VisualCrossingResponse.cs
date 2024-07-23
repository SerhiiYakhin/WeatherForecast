namespace WeatherForecast.Models.WeatherApiResponses.VisualCrossing;

public record VisualCrossingResponse
{
    public string address { get; set; }
    public string resolvedAddress { get; set; }
    public List<VisualCrossingDay> days { get; set; }
}