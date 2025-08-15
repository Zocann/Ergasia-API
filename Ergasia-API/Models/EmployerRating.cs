using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Models;

public class EmployerRating : BaseRating
{
    public EmployerRating() { }

    public EmployerRating(long id, Employer employer, Worker worker, int numRating, string? verRating) : base(id, numRating, verRating)
    {
        Employer = employer;
        EmployerId = employer.Id;
        Worker = worker;
        WorkerId = worker.Id;
        Date = DateTime.Now;
    }
    
    [Required]
    public Employer Employer { get; set; }
    [Required]
    public int EmployerId { get; set; }
    
    [Required]
    public Worker Worker { get; set; }
    [Required]
    public int WorkerId { get; set; }
    public DateTime Date { get; set; }
}