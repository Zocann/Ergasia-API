namespace Ergasia_API.Models;

public class Employer : User
{ 
    public string? CompanyName { get; set; }
    public string? CompanyState { get; set; }
    public string? CompanyCity { get; set; }
    public string? CompanyAddress { get; set; }
}