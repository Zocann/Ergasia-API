using Ergasia_API.Attributes;

namespace Ergasia_API.Models;

public class Job
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required int Salary { get; set; }
    public string? Description { get; set; }
    
    [ValidJobDate]
    public required DateTime DateOfBegin { get; set; }
    public required int Duration { get; set; }
    public required int WorkSpots { get; set; }
    public required string EmployerId { get; set; }
    
    //Navigation property
    public required Employer Employer { get; set; }
}