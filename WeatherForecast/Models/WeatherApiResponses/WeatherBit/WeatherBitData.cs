namespace WeatherForecast.Models.WeatherApiResponses.WeatherBit;

public record WeatherBitData
{
    public string datetime { get; set; }
    public double temp { get; set; }
    public double min_temp { get; set; }
    public double max_temp { get; set; }
}