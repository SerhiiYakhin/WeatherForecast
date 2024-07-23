using WeatherForecast.ModelBinders;
using WeatherForecast.Models;
using WeatherForecast.Services;

namespace WeatherForecast;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        builder.Services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new DateModelBinderProvider());
        });
        builder.Services.AddHttpClient();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IWeatherService, WeatherService>();
        builder.Services.Decorate<IWeatherService, CachedWeatherService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<ApiKeys>(builder.Configuration.GetSection("APIKeys"));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.Run();
    }
}