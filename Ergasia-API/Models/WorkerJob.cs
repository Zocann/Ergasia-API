using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Models;

public class WorkerJob
{
    WorkerJob() {}

    WorkerJob(int workerId, long jobId, Worker worker, Job job)
    {
        WorkerId = workerId;
        JobId = jobId;
        Worker = worker;
        Job = job;
    }

    public long Id { get; set; }
    [Required]
    public int WorkerId { get; set; }
    [Required]
    public long JobId { get; set; }
    public int? NumericalRating { get; set; } = null;
    [StringLength(30, ErrorMessage = "Rating cannot be longer than 30 characters")]
    public string? VerbalRating { get; set; } = null;

    // Navigation
    [Required]
    public Worker Worker { get; set; }
    [Required]
    public Job Job { get; set; }
}