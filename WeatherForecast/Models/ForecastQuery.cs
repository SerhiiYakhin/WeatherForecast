using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Attributes;
using WeatherForecast.ModelBinders;

namespace WeatherForecast.Models;

public class ForecastQuery
{
    [FromQuery(Name = "date")]
    [ModelBinder(BinderType = typeof(DateModelBinder))]
    [ValidDateRange]
    [Required(ErrorMessage = "The 'date' parameter is required.")]
    public DateTime Date { get; set; }

    [FromQuery(Name = "city")]
    [Required(ErrorMessage = "The 'city' parameter is required.")]
    public string City { get; set; }

    [FromQuery(Name = "country")]
    [Required(ErrorMessage = "The 'country' parameter is required.")]
    public string Country { get; set; }
}