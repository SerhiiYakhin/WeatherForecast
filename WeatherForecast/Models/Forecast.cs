namespace WeatherForecast.Models;

public record Forecast(
    DateTime Date,
    string City,
    string Country,
    string Source,
    string? Description,
    Temperature Temperature);