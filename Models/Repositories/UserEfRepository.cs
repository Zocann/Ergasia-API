using AutoMapper;
using Ergasia_API.DTOs.User;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class UserEfRepository(UserManager<User> userManager, IMapper mapper, ITokenService tokenService) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id)
    {
        return await userManager.FindByIdAsync(id);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return userManager.FindByEmailAsync(email);
    }

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        return await userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto, string userType)
    {
        IdentityResult result;
        
        switch (userType)
        {
            case "Worker":
                var worker = mapper.Map<Worker>(registerDto);
                worker.UserName = worker.Email?.ToLowerInvariant();
                result = await userManager.CreateAsync(worker, registerDto.Password);
                if(result.Succeeded) await userManager.AddToRolesAsync(worker, new List<string> { "Worker" });
                return result;
            
            case "Employer":
                var employer = mapper.Map<Employer>(registerDto);
                employer.UserName = employer.Email?.ToLowerInvariant();
                result = await userManager.CreateAsync(employer, registerDto.Password);
                if(result.Succeeded) await userManager.AddToRolesAsync(employer, new List<string> { "Employer" });
                return result;
            
            default:
                return IdentityResult.Failed();
        }
    }

    public async Task<IdentityResult?> AddRolesAsync(User user, List<string> roles)
    {
        return await userManager.AddToRolesAsync(user, roles);
    }

    public async Task<User?> LoginAsync(LoginDto loginDto)
    {
        var account = await userManager.FindByNameAsync(loginDto.Email.ToLowerInvariant());

        if (account != null && await userManager.CheckPasswordAsync(account, loginDto.Password)) return account;
        
        return null;
    }

    public async Task<IdentityResult?> UpdateAsync(User user)
    {
        if (await GetByIdAsync(user.Id) == null) return null;
        
        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult?> DeleteAsync(string id)
    {
        var account = await userManager.FindByIdAsync(id);

        if (account == null) return null;

        return await userManager.DeleteAsync(account);
    }

    public async Task<User> SetRefreshToken(User user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(3);
        
        await userManager.UpdateAsync(user);

        return user;
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await userManager.Users.FirstOrDefaultAsync(a =>
            a.RefreshToken == refreshToken && a.RefreshTokenExpiration > DateTime.UtcNow);
    }
}