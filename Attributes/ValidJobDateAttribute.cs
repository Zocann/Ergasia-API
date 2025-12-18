using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Attributes;

public class ValidJobDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dateTime = ConvertObjectToDateTime(value);

        return dateTime > DateTime.UtcNow
            ? ValidationResult.Success
            : new ValidationResult("Date of begin cannot be in the past");
    }

    private static DateTime ConvertObjectToDateTime(object? value)
    {
        return Convert.ToDateTime(value);
    }
}