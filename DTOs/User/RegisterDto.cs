using System.ComponentModel.DataAnnotations;
using Ergasia_API.Attributes;

namespace Ergasia_API.DTOs.User;

public class RegisterDto(string firstName, string lastName, string password, string email, string phoneNumber, string state, string city, string address, DateTime dateOfBirth)
{
    [Required(ErrorMessage = "Please enter your first name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = firstName;
    
    [Required(ErrorMessage = "Please enter your last name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = lastName;

    [Required (ErrorMessage = "Please enter password")]
    [StringLength(16, MinimumLength = 4, ErrorMessage = "Must be between 4 and 16 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = password;
    
    [EmailAddress] [Required]
    public string Email { get; set; } = email;
    
    [Phone] [Required]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = phoneNumber;
    
    [Required(ErrorMessage = "Please enter state")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "State must be between 3 and 15 characters")]
    public string State { get; set; } = state;
    
    [Required(ErrorMessage = "Please enter city")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "City must be between 3 and 15 characters")]
    public string City { get; set; } = city;
    
    [Required(ErrorMessage = "Please enter address")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 30 characters")]
    public string Address { get; set; } = address;
    
    [Required] 
    public DateTime DateOfBirth { get; set; } = dateOfBirth;
}