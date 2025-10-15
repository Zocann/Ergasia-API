namespace Ergasia_API.Models;

public class Worker : User
{
    public int? MinimalSalary { get; set; }
    public string? Description { get; set; }
}