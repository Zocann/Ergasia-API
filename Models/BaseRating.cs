using System.ComponentModel.DataAnnotations.Schema;

namespace Ergasia_API.Models;

public abstract class BaseRating
{
    public required int NumericalRating { get; set; }
    public required string? VerbalRating { get; set; }
    
    [ForeignKey("Employer")]
    public required string EmployerId { get; set; }
    
    [ForeignKey("Worker")]
    public required string WorkerId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    
    //Navigational
    public required Employer Employer { get; set; }
    
    public required Worker Worker { get; set; }
}