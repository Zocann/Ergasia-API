using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Models;

public class EmployerJob
{
    public EmployerJob() { }
    
    EmployerJob(int employerId, long jobId, Employer employer, Job job)
    {
        EmployerId = employerId;
        JobId = jobId;
        Employer = employer;
        Job = job;
    }
    
    public long Id { get; set; }
    [Required]
    public int EmployerId { get; set; }
    [Required]
    public long JobId { get; set; }

    // Navigation
    [Required]
    public Employer Employer { get; set; }
    [Required]
    public Job Job { get; set; }
}