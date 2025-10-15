using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ergasia_API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.DTOs.Job;

public class JobDto
{
    [HiddenInput]
    public string? Id { get; set; }
    
    [Required(ErrorMessage = "Please enter job name")]
    [StringLength(25, ErrorMessage = "Must be between 3 and 25 characters", MinimumLength = 3)]
    [Display(Name = "Job name")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Please enter salary")]
    [Display(Name = "Monthly salary")]
    public required int Salary { get; set; }
    
    [Display(Name = "Job description")]
    [StringLength(255, ErrorMessage = "Cannot exceed 255 characters")]
    public string? Description { get; set; }
    
    [ValidJobDate]
    public DateTime DateOfBegin { get; set; }
    
    [Description("Duration in days")]
    [Range(1, 365, ErrorMessage = "Job duration cannot be negative or longer then 1 year")]
    [Required(ErrorMessage = "Please enter duration in days")]
    public int Duration { get; set; }
    
    [Description("Number of left spots for workers")]
    [Range(1, 50, ErrorMessage = "Number of work spots cannot be negative or higher then 50")]
    [Required(ErrorMessage = "Please enter number of spots for workers")]
    public int WorkSpots { get; set; }
    
    [HiddenInput]
    public string? EmployerId { get; set; }
}