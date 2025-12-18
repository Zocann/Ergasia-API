using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Ergasia_API.Services;

public class TokenService(IUserRepository userRepository, IConfiguration config) : ITokenService
{
    public async Task<string> GenerateAccessToken(User user)
    {
        var tokenKey = GetTokenKey();

        if (!IsValidTokenKey(tokenKey)) throw new Exception("Token key needs to be at least 64 characters long");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = BuildClaimsFromUser(user);
        var role = await GetRoleFromUserRepositoryAsync(user);
        if (string.IsNullOrEmpty(role)) throw new Exception("User has no role");

        claims.Add(new Claim(ClaimTypes.Role, role));

        var credentials = BuildSigningCredentials(key);
        var tokenDescriptor = BuildSecurityTokenDescriptor(claims, credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<bool> SetRefreshTokenAsync(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(3);

        return await UpdateRefreshTokenInUserRepositoryAsync(user);
    }

    private string GetTokenKey()
    {
        return Environment.GetEnvironmentVariable("TOKEN_KEY") ?? throw new Exception("Token key not found");
        //return config["TokenKey"] ?? throw new Exception("Token key not found");
    }

    private static bool IsValidTokenKey(string tokenKey)
    {
        return tokenKey.Length > 64;
    }

    private static List<Claim> BuildClaimsFromUser(User user)
    {
        return
        [
            new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException("User email is null")),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        ];
    }

    private async Task<string?> GetRoleFromUserRepositoryAsync(User user)
    {
        var roles = await userRepository.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }

    private static SigningCredentials BuildSigningCredentials(SymmetricSecurityKey key)
    {
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
    }

    private static SecurityTokenDescriptor BuildSecurityTokenDescriptor(List<Claim> claims,
        SigningCredentials signingCredentials)
    {
        const int expirationInMinutes = 1;
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            SigningCredentials = signingCredentials
        };
    }

    private async Task<bool> UpdateRefreshTokenInUserRepositoryAsync(User user)
    {
        var result = await userRepository.UpdateAsync(user);
        return result.Succeeded;
    }
}