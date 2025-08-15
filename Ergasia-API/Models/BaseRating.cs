using System.ComponentModel.DataAnnotations;

namespace Ergasia_API.Models;

public abstract class BaseRating
{
    protected BaseRating() { }

    protected BaseRating(long id, int numRating, string? verRating)
    {
        Id = id;
        NumericalRating = numRating;
        VerbalRating = verRating;
    }
    
    [Required]
    public long Id { get; set; }
    
    [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
    [Required]
    public int NumericalRating { get; set; }
    
    [StringLength(255, ErrorMessage = "Cannot exceed 255 characters")]
    public string? VerbalRating { get; set; }
}