namespace WeatherForecast.Models.WeatherApiResponses.WeatherBit;

public record WeatherBitResponse
{
    public string city_name { get; set; }
    public string country_code { get; set; }
    public List<WeatherBitData> data { get; set; }
}