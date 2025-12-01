namespace Ergasia_API.Data;

public struct RatingDto
{
    public string EmployerId { get; set; }
    public string WorkerId { get; set; }
    public int NumericalRating { get; set; }
    public string? VerbalRating { get; set; }
}