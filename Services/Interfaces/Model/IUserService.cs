using Ergasia_API.Data;
using Ergasia_API.DTOs.User;
using Ergasia_API.Models;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IUserService
{
    public Task<ServiceResult<UserDto>> GetAsync(string id);
    public Task<ServiceResult<string>> GetSingleRoleAsync(string id);
    public Task<ServiceResult<bool>> UserWithThisEmailExists(string email);
    public Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string userType);
    public Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);
    public Task<ServiceResult<UserDto>> UpdateAsync(UpdateUserDto updateUserDto);
    public Task<ServiceResult<bool>> UpdateRefreshTokenAsync(User user);
    public Task<ServiceResult<bool>> UploadPictureAsync(IFormFile file, string id);
    public Task<ServiceResult<bool>> DeleteAsync(string id);
    public Task<ServiceResult<UserDto>> GetRefreshTokenAsync(string refreshToken);
}