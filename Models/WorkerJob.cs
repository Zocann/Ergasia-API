using System.ComponentModel.DataAnnotations.Schema;

namespace Ergasia_API.Models;

public class WorkerJob
{
    [ForeignKey("Worker")]
    public required string WorkerId { get; set; }
    
    [ForeignKey("Job")]
    public required string JobId { get; set; }
    public int? NumericalRating { get; set; }
    public string? VerbalRating { get; set; }
    public DateTime? DateOfRating { get; set; }

    // Navigation
    public required Worker Worker { get; set; }
    public required Job Job { get; set; }
}