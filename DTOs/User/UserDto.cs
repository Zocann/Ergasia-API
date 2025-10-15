namespace Ergasia_API.DTOs.User;

public class UserDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public required string State { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required DateTime DateOfRegistration { get; set; }
    public string? PictureUrl { get; set; }
    //Security
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
}