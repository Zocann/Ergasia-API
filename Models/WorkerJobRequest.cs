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
    public Worker Worker { get; set; }
    public Job Job { get; set; }
}