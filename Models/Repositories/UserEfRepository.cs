using AutoMapper;
using Ergasia_API.DTOs.User;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class UserEfRepository(UserManager<User> userManager, IMapper mapper) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id)
    {
        return await userManager.FindByIdAsync(id);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return userManager.FindByEmailAsync(email);
    }

    public async Task<IEnumerable<string>> GetRolesAsync(User user)
    {
        return await userManager.GetRolesAsync(user);
    }
    
    public async Task<IdentityResult> AddWorkerAsync(Worker worker, string password)
    {
        return await userManager.CreateAsync(worker, password);
    }

    public async Task<IdentityResult> AddEmployerAsync(Employer employer, string password)
    {
        return await userManager.CreateAsync(employer, password);
    }

    public async Task<IdentityResult> AddRolesAsync(User user, List<string> roles)
    {
        return await userManager.AddToRolesAsync(user, roles);
    }

    public async Task<IdentityResult> UpdateAsync(User user)
    {
        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(User account)
    {
        return await userManager.DeleteAsync(account);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await userManager.Users.FirstOrDefaultAsync(a =>
            a.RefreshToken == refreshToken && a.RefreshTokenExpiration > DateTime.UtcNow);
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await userManager.CheckPasswordAsync(user, password);
    }
}