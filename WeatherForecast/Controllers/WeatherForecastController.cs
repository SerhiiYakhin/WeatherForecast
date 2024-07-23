using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Models;
using WeatherForecast.Services;

namespace WeatherForecast.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(IWeatherService weatherService) : ControllerBase
{
    /// <summary>
    ///     Gets the weather forecast for a specific date, city, and country.
    /// </summary>
    /// <param name="query">The query parameters for the weather forecast.</param>
    /// <returns>A collection of weather forecasts from various sources.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Forecast>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetForecast([FromQuery] ForecastQuery query)
    {
        if (!ModelState.IsValid) return BadRequest(ValidationProblem(ModelState));

        var forecasts = await weatherService.GetForecastsAsync(query.Date, query.City, query.Country);
        return Ok(forecasts);
    }
}