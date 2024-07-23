namespace WeatherForecast.Models.WeatherApiResponses.VisualCrossing;

public record VisualCrossingDay
{
    public string datetime { get; set; }
    public double temp { get; set; }
    public double tempmin { get; set; }
    public double tempmax { get; set; }
    public string conditions { get; set; }
}