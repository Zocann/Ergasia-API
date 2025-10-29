using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Exception = System.Exception;

namespace Ergasia_API.Services;

public class TokenService(IUserRepository userRepository) : ITokenService
{
    public async Task<string> GenerateAccessToken(User user)
    {
        var tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY") ?? throw new Exception("Token key not found");
        if (tokenKey.Length < 64) throw new Exception("Token key needs to be at least 64 characters long");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException("User email is null")),
            new(ClaimTypes.NameIdentifier, user.Id),
        };
        var roles = await userRepository.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
    
    public async Task<bool> SetRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(3);

        var result = await userRepository.UpdateAsync(user);
        
        if(result != null && result.Succeeded) return true;
        return false;
    }
}