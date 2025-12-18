using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Attributes;

//Attribute for date of birth validation
public class ValidDateOfBirthAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dateOfBirth = ConvertObjectToDateTime(value);
        if (! IsValidDateOfBirth(dateOfBirth)) return new ValidationResult("Invalid Date Of Birth.");
        return DateOfBirthYearIsUnder(dateOfBirth, 18)
            ? ValidationResult.Success 
            : new ValidationResult("Worker needs to be at least 18 years old");
    }

    private static DateTime ConvertObjectToDateTime(object? value)
    {
        return Convert.ToDateTime(value);
    }

    private static bool IsValidDateOfBirth(DateTime dateOfBirth)
    {
        return dateOfBirth >= DateTime.Now;
    }

    private static bool DateOfBirthYearIsUnder(DateTime dateOfBirth, int year)
    {
        return dateOfBirth <= DateTime.Now.AddYears(-year);
    }
}