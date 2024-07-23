using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using WeatherForecast.Models.WeatherApiResponses.VisualCrossing;
using WeatherForecast.Models.WeatherApiResponses.WeatherApi;
using WeatherForecast.Models.WeatherApiResponses.WeatherBit;
using WeatherForecast.Services;

namespace WeatherForecast.Tests;

public class WeatherServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly ILogger<WeatherService> _logger;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _configurationMock = new Mock<IConfiguration>();
        _logger = NullLogger<WeatherService>.Instance;
        _weatherService = new WeatherService(_httpClientFactoryMock.Object, _configurationMock.Object, _logger);
    }

    [Fact]
    public async Task GetWeatherForecastsAsync_ReturnsForecasts()
    {
        // Arrange
        var date = new DateTime(2024, 1, 1);
        var city = "London";
        var country = "UK";

        var weatherApiResponse = new WeatherApiResponse
        {
            location = new Location { name = city, country = country },
            forecast = new Forecast
            {
                forecastday =
                [
                    new ForecastDay
                    {
                        date = date.ToString("yyyy-MM-dd"),
                        day = new Day
                        {
                            avgtemp_c = 10,
                            mintemp_c = 5,
                            maxtemp_c = 15,
                            condition = new Condition { text = "Sunny" }
                        }
                    }
                ]
            }
        };

        var weatherBitResponse = new WeatherBitResponse
        {
            data =
            [
                new WeatherBitData
                {
                    datetime = date.ToString("yyyy-MM-dd"),
                    temp = 10,
                    min_temp = 5,
                    max_temp = 15
                }
            ]
        };

        var visualCrossingResponse = new VisualCrossingResponse
        {
            days =
            [
                new VisualCrossingDay
                {
                    datetime = date.ToString("yyyy-MM-dd"),
                    temp = 10,
                    tempmin = 5,
                    tempmax = 15,
                    conditions = "Sunny"
                }
            ]
        };

        var weatherApiHandlerMock = new Mock<HttpMessageHandler>();
        weatherApiHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("weatherapi")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(weatherApiResponse)
            });

        var weatherBitHandlerMock = new Mock<HttpMessageHandler>();
        weatherBitHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("weatherbit")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(weatherBitResponse)
            });

        var visualCrossingHandlerMock = new Mock<HttpMessageHandler>();
        visualCrossingHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("visualcrossing")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(visualCrossingResponse)
            });

        var weatherApiClient = new HttpClient(weatherApiHandlerMock.Object);
        var weatherBitClient = new HttpClient(weatherBitHandlerMock.Object);
        var visualCrossingClient = new HttpClient(visualCrossingHandlerMock.Object);

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.Is<string>(n => n.Contains("WeatherApi"))))
            .Returns(weatherApiClient);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.Is<string>(n => n.Contains("WeatherBit"))))
            .Returns(weatherBitClient);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.Is<string>(n => n.Contains("VisualCrossing"))))
            .Returns(visualCrossingClient);

        _configurationMock.Setup(c => c["APIKeys:WeatherApiKey"]).Returns("weather-api-key");
        _configurationMock.Setup(c => c["APIKeys:WeatherBit"]).Returns("weatherbit-api-key");
        _configurationMock.Setup(c => c["APIKeys:VisualCrossing"]).Returns("visualcrossing-api-key");

        // Act
        var forecasts = await _weatherService.GetForecastsAsync(date, city, country);

        // Assert
        Assert.NotNull(forecasts);
        Assert.Equal(3, forecasts.Count());
    }

    [Fact]
    public async Task GetWeatherForecastsAsync_HandlesNonSuccessStatusCode()
    {
        // Arrange
        var date = new DateTime(2024, 1, 1);
        var city = "London";
        var country = "UK";

        var weatherApiHandlerMock = new Mock<HttpMessageHandler>();
        weatherApiHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("weatherapi")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var weatherBitHandlerMock = new Mock<HttpMessageHandler>();
        weatherBitHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("weatherbit")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var visualCrossingHandlerMock = new Mock<HttpMessageHandler>();
        visualCrossingHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("visualcrossing")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var weatherApiClient = new HttpClient(weatherApiHandlerMock.Object);
        var weatherBitClient = new HttpClient(weatherBitHandlerMock.Object);
        var visualCrossingClient = new HttpClient(visualCrossingHandlerMock.Object);

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.Is<string>(n => n.Contains("WeatherApi"))))
            .Returns(weatherApiClient);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.Is<string>(n => n.Contains("WeatherBit"))))
            .Returns(weatherBitClient);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.Is<string>(n => n.Contains("VisualCrossing"))))
            .Returns(visualCrossingClient);

        _configurationMock.Setup(c => c["APIKeys:WeatherApiKey"]).Returns("weather-api-key");
        _configurationMock.Setup(c => c["APIKeys:WeatherBit"]).Returns("weatherbit-api-key");
        _configurationMock.Setup(c => c["APIKeys:VisualCrossing"]).Returns("visualcrossing-api-key");

        // Act
        var forecasts = await _weatherService.GetForecastsAsync(date, city, country);

        // Assert
        Assert.NotNull(forecasts);
        Assert.Empty(forecasts);
    }
}