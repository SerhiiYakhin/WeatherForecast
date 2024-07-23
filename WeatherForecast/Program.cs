using WeatherForecast.ModelBinders;
using WeatherForecast.Services;

namespace WeatherForecast;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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