using System.ComponentModel.DataAnnotations;
using Ergasia_API.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Ergasia_API.Models;

public class User: IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public required string State { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    
    [ValidDateOfBirth]
    public required DateTime DateOfBirth { get; set; }

    public required DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;
    
    [Url]
    public string? PictureUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}