using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.DTOs.User;

public class UpdateUserDto(string id, string firstName, string lastName, string phoneNumber, string state, string city, string address)
{
    [HiddenInput] 
    public string Id { get; set; } = id;
    
    [Required(ErrorMessage = "Please enter your first name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = firstName;
    
    [Required(ErrorMessage = "Please enter your last name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = lastName;
    
    [Phone] [Required]
    public string PhoneNumber { get; set; } = phoneNumber;
    
    [Required(ErrorMessage = "Please enter state, where your company is located")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "State must be between 3 and 15 characters")]
    public string State { get; set; } = state;
    
    [Required(ErrorMessage = "Please enter city, where your company is located")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "City must be between 3 and 15 characters")]
    public string City { get; set; } = city;
    
    [Required(ErrorMessage = "Please enter state, where your company is located")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 30 characters")]
    public string Address { get; set; } = address;
}