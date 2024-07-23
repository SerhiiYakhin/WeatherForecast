using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherForecast.Controllers;
using WeatherForecast.Models;
using WeatherForecast.Services;

namespace WeatherForecast.Tests;

public class WeatherForecastControllerTests
{
    private readonly WeatherForecastController _controller;
    private readonly Mock<IWeatherService> _weatherServiceMock;

    public WeatherForecastControllerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _controller = new WeatherForecastController(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task GetForecast_ReturnsOkResult_WithForecasts()
    {
        // Arrange
        var date = new DateTime(2024, 1, 1);
        var city = "London";
        var country = "UK";

        var forecasts = new List<Forecast>
        {
            new(date, city, country, "WeatherAPI", "Sunny", new Temperature(10, 5, 15)),
            new(date, city, country, "WeatherBit", "Sunny", new Temperature(10, 5, 15)),
            new(date, city, country, "VisualCrossing", "Sunny", new Temperature(10, 5, 15))
        };

        _weatherServiceMock.Setup(s => s.GetForecastsAsync(date, city, country)).ReturnsAsync(forecasts);

        var query = new ForecastQuery { Date = date, City = city, Country = country };

        // Act
        var result = await _controller.GetForecast(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Forecast>>(okResult.Value);
        Assert.Equal(3, returnValue.Count);
    }

    [Fact]
    public async Task GetForecast_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Date", "Required");

        var query = new ForecastQuery { Date = default, City = "London", Country = "UK" };

        // Act
        var result = await _controller.GetForecast(query);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Theory]
    [InlineData("2022-01-01")] // Date too far in the past
    [InlineData("2025-01-01")] // Date too far in the future
    public async Task GetForecast_ReturnsBadRequest_WhenDateRangeIsInvalid(string dateString)
    {
        // Arrange
        var date = DateTime.Parse(dateString);
        _controller.ModelState.AddModelError("Date", "Required");

        var query = new ForecastQuery { Date = date, City = "London", Country = "UK" };

        // Act
        var result = await _controller.GetForecast(query);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
}