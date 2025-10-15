using Ergasia_API.Models;

namespace Ergasia_API.Services.Interfaces;

public interface ITokenService
{
    public Task<string> GenerateToken(User user);
    public string GenerateRefreshToken();
}