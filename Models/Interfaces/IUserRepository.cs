using Ergasia_API.DTOs.User;
using Microsoft.AspNetCore.Identity;

namespace Ergasia_API.Models.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByIdAsync(string id);
    public Task<User?> GetByEmailAsync(string email);
    public Task<IEnumerable<string>> GetRolesAsync(User user);
    public Task<IdentityResult> AddWorkerAsync(Worker worker, string password);
    public Task<IdentityResult> AddEmployerAsync(Employer employer, string password);
    public Task<IdentityResult> AddRolesAsync(User user, List<string> roles);
    public Task<IdentityResult> UpdateAsync(User user);
    public Task<IdentityResult> DeleteAsync(User user);
    public Task<User?> GetByRefreshTokenAsync(string refreshToken);
    public Task<bool> CheckPasswordAsync(User user, string loginDtoPassword);
}