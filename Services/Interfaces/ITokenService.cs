using Ergasia_API.Models;

namespace Ergasia_API.Services.Interfaces;

public interface ITokenService
{
    public Task<string> GenerateAccessToken(User user);
    public string GenerateRefreshToken();
    public Task<bool> SetRefreshTokenAsync(User user);
}