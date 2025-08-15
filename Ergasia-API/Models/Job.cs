using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ergasia_API.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Models;

public class Job
{
    public Job() { }
    
    public Job(string name, DateTime dateOfBegin, int duration, int workSpots)
    {
        Name = name;
        DateOfBegin = dateOfBegin;
        Duration = duration;
        AvilableWorkSpots = workSpots;
    }
    
    [HiddenInput]
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Please enter job name")]
    [StringLength(25, ErrorMessage = "Must be between 3 and 25 characters", MinimumLength = 3)]
    [Display(Name = "Job name")]
    public string Name { get; set; }
    
    [Display(Name = "Job description")]
    [StringLength(255, ErrorMessage = "Cannot exceed 255 characters")]
    public string? Description { get; set; }
    
    [ValidJobDate]
    [Required(ErrorMessage = "Please enter date of begin")]
    public DateTime DateOfBegin { get; set; }
    
    [Description("Duration in days")]
    [MaxLength(365, ErrorMessage = "Job duration cannot be longer then 1 year")]
    [Required(ErrorMessage = "Please enter duration in days")]
    public int Duration { get; set; }
    
    [Description("Weather the job is currently active")]
    public bool IsCurrent { get; set; }
    
    [Description("Number of left spots for workers")]
    [Required(ErrorMessage = "Please enter number of spots for workers")]
    public int AvilableWorkSpots { get; set; }
}