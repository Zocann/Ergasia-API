using Ergasia_API.DTOs.Employer;
using Ergasia_API.DTOs.Worker;

namespace Ergasia_API.DTOs.Rating;

public class EmployerRatingDto
{
    public required EmployerDto EmployerDto { get; set; }
    public required WorkerDto WorkerDto { get; set; }
    public required int NumericalRating { get; set; }
    public string? VerbalRating { get; set; }
    public required DateTime Date { get; set; }
}