using Ergasia_API.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace Ergasia_API.Models.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByIdAsync(string id);
    public Task<User?> GetByEmailAsync(string email);
    public Task<IList<string>> GetRolesAsync(User user);
    public Task<IdentityResult> RegisterAsync(RegisterDto registerDto, string userType);
    public Task<User?> LoginAsync(LoginDto loginDto);
    public Task<IdentityResult?> AddRolesAsync(User user, List<string> roles);
    public Task<IdentityResult?> UpdateAsync(User user);
    public Task<IdentityResult?> DeleteAsync(string id);


    public Task<User> SetRefreshToken(User user);
    public Task<User?> GetByRefreshTokenAsync(string refreshToken);
}