using Ergasia_API.Data;
using Ergasia_API.DTOs.User;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IUserService
{
    public Task<ServiceResult<UserDto>> GetAsync(string id);
    public Task<ServiceResult<string>> GetSingleRoleAsync(string id);
    public Task<ServiceResult<bool>> UserWithThisEmailExists(string email);
    public Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string userType);
    public Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto);
    public Task<ServiceResult<UserDto>> UpdateAsync(UpdateUserDto updateUserDto);
    public Task<ServiceResult<bool>> UploadPictureAsync(IFormFile file, string id);
    public Task<ServiceResult<bool>> DeleteAsync(string id);
    public Task<ServiceResult<UserDto>> GetRefreshTokenAsync(string refreshToken);
}