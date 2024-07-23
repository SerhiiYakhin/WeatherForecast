using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Attributes;

public class ValidDateRangeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime date || date == default)
            return new ValidationResult(
                $"The '{validationContext.DisplayName.ToLowerInvariant()}' parameter is absent or not a valid date.");

        var today = DateTime.Today;
        var oneYearAgo = today.AddDays(-365);

        if (date >= oneYearAgo && date <= today.AddDays(7))
            return ValidationResult.Success;
        return new ValidationResult(
            $"The '{validationContext.DisplayName.ToLowerInvariant()}' parameter must be within the last 365 days or next 7 days.");
    }
}