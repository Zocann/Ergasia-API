using System.ComponentModel.DataAnnotations.Schema;

namespace Ergasia_API.Models;

public class WorkerJobRequest
{
    [ForeignKey("Worker")]
    public required string WorkerId { get; set; }
    
    [ForeignKey("Job")]
    public required string JobId { get; set; }
    public required DateTime ExpirationDate { get; set; }
    public string? Message { get; set; }
    
    // Navigation
    public required Worker Worker { get; set; }
    public required Job Job { get; set; }
}