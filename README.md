# Weather Forecast Service

This is a .NET Core microservice that provides weather forecasts from multiple sources. The service fetches weather data from various APIs and aggregates the results.

## Features

- Fetches weather data from multiple sources:
  - Weather API
  - WeatherBit API
  - Visual Crossing API
- Supports date validation and custom model binding
- Dockerized for easy deployment
- Unit tests with xUnit and Moq

## Prerequisites

- .NET 9.0 SDK
- Docker (for containerization)

## Getting Started

### Clone the Repository

```sh
git clone https://github.com/SerhiiYakhin/WeatherForecast.git
cd WeatherForecast
```

### Configuration

Add your API keys to the appsettings.json file:

```json
{ "APIKeys": { "WeatherApiKey": "your-weather-api-key", "WeatherBit": "your-weatherbit-api-key", "VisualCrossing": "your-visualcrossing-api-key" } }
```

### Build and Run the Application

#### Using .NET CLI

```sh
dotnet build
dotnet run
```

#### Using Docker

1. Build the Docker image:

```sh
docker build -t weather-forecast . -f .\WeatherForecast\Dockerfile
```

2. Run the Docker container:

```sh
docker run -d -p 32774:8081 -p 32775:8080 --name weather-forecast weather-forecast
```

## API Endpoints

### Get Weather Forecast

- Endpoint: `/weatherforecast`
- Method: `GET`
- Query Parameters:
  - `date` (required): The date for the weather forecast (supports multiple formats: `yyyy-MM-dd`, `dd-MM-yyyy`, etc.)
    - Current version support history data from 365 day ago and forecast up to 7 days ahead.
  - `city` (required): The city for the weather forecast
  - `country` (required): The country for the weather forecast

#### Example request

```sh
curl "https://localhost:32774/weatherforecast?date=2024-01-01&city=London&country=UK"
```

## Running Tests

Run the unit tests using the .NET CLI:

```sh
dotnet test
```
