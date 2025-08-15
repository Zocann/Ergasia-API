using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Attributes;

public class ValidJobDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        DateTime d = Convert.ToDateTime(value);

        return d > DateTime.Now
            ? ValidationResult.Success
            : new ValidationResult("Date of begin cannot be in the past");
    }
}