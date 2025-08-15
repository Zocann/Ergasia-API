using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Models;

public class Employer
{
    public Employer(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }
    
    [HiddenInput]
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter your company name")]
    [StringLength(25, ErrorMessage = "Must be between 3 and 25 characters", MinimumLength = 3)]
    [Display(Name = "Company name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(30, ErrorMessage = "Email must be longer then 7 characters", MinimumLength = 7)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(30, ErrorMessage = "Password must be longer then 5 characters", MinimumLength = 5)]
    public string Password { get; set; }

    [Description("Wether the employer is active on Ergasia platform")]
    public bool IsActive { get; set; } = true;
    
    [Url]
    [StringLength(50, ErrorMessage = "Url cannot be longer than 50 characters")]
    public string? PictureUrl { get; set; }
}