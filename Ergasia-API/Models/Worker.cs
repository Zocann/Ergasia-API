using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ergasia_API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Models;

public class Worker
{
    
    public Worker(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }
    
    [HiddenInput]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Please enter your first name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Please enter your last name")]
    [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(30, ErrorMessage = "Password must be longer then 5 characters", MinimumLength = 7)]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(30, ErrorMessage = "Password must be longer then 5 characters", MinimumLength = 5)]
    public string Password { get; set; }
    
    [Description("Wether the user is active on this Ergasia platform")]
    public bool IsActive { get; set; }
    
    [ValidDateOfBirth]
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Url]
    [StringLength(50, ErrorMessage = "Cannot exceed 50 characters")]
    public string? PictureUrl { get; set; }
    
    [Description("Minimal salary set by worker")]
    public int? MinimalSalary { get; set; }
}