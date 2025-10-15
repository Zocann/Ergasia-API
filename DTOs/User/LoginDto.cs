using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.DTOs.User;

public class LoginDto(string email, string password)
{
    [Required]
    [Display(Name = "Email address")]
    [EmailAddress]
    public string Email { get; set; } = email;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = password;
}