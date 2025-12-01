using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.User;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class UserService(IUserRepository repository, ITokenService tokenService, 
    IProfilePictureService profilePictureService, IMapper mapper) : IUserService
{
    public async Task<ServiceResult<UserDto>> GetAsync(string id)
    {
        try
        {
            var user = await GetUserFromRepositoryAsync(id);

            if (user == null)
                return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.NotFound);

            var userDto = MapUserToDto(user);
            
            return ServiceResultBuilder.BuildSuccess(userDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<string>> GetSingleRoleAsync(string id)
    {
        try
        {
            var user = await GetUserFromRepositoryAsync(id);
            if (user == null)
                return ServiceResultBuilder.BuildFailure<string>(ServiceResultError.DependenciesNotFound);
            
            var roles = (await repository.GetRolesAsync(user)).ToList();

            return roles.Count == 0 ? 
                ServiceResultBuilder.BuildFailure<string>(ServiceResultError.EmptyCollection) : 
                ServiceResultBuilder.BuildSuccess(roles.FirstOrDefault());
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<string>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> UserWithThisEmailExists(string email)
    {
        try
        {
            var user = await repository.GetByEmailAsync(email);

            return user == null ? 
                ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.NotFound) :
                ServiceResultBuilder.BuildSuccess(true);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string userType)
    {
        try
        {
            User user;
            switch (userType)
            {
                case "Worker":
                    var worker = await RegisterWorker(registerDto);
                    if (worker == null)
                        return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
                    user = worker;
                    break;
                
                case "Employer":
                    var employer = await RegisterEmployer(registerDto);
                    if (employer == null)
                        return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
                    user = employer;
                    break;
            
                default:
                    return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.InvalidArgument);
            }
            
            var userDto = MapUserToDto(user);
            userDto.AccessToken = await tokenService.GenerateAccessToken(user);
            return ServiceResultBuilder.BuildSuccess(MapUserToDto(user));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await repository.GetByEmailAsync(loginDto.Email.ToLowerInvariant());
            if (user == null) return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.NotFound);
            
            var resultIsSuccess = await repository.CheckPasswordAsync(user, loginDto.Password);

            if (! resultIsSuccess) ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.InvalidCredentials);
            
            if (! await SetRefreshTokenAsync(user))
                return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.TokenError);

            var userDto = MapUserToDto(user);
            userDto.AccessToken = await tokenService.GenerateAccessToken(user);
            
            return ServiceResultBuilder.BuildSuccess(userDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<UserDto>> UpdateAsync(UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await GetUserFromRepositoryAsync(updateUserDto.Id);
            if (user == null) return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.NotFound);
            
            var newUser = mapper.Map(updateUserDto, user);
            var result = await repository.UpdateAsync(newUser);
            
            return result.Succeeded ?
                ServiceResultBuilder.BuildSuccess(MapUserToDto(newUser)) :
                ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> UploadPictureAsync(IFormFile file, string id)
    {
        try
        {
            var user = await GetUserFromRepositoryAsync(id);
            if (user == null) return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DependenciesNotFound);
            
            var extension = GetExtesion(file.FileName);
            if (string.IsNullOrEmpty(extension)) return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.InvalidArgument);
        
            var fileName = BuildFileName(id, extension);
            
            var url = await UploadProfilePictureAndGetUrlAsync(file, fileName, extension);
            var result = await UploadUrlToRepositoryAsync(url, user);
            
            return result ? 
                ServiceResultBuilder.BuildSuccess(true) :
                ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.InvalidArgument);

        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string id)
    {
        try
        {
            var user = await GetUserFromRepositoryAsync(id);
            if (user == null) return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.NotFound);
            
            var result = await repository.DeleteAsync(user);
            
            return result.Succeeded ? 
                ServiceResultBuilder.BuildSuccess(true) :
                ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);

        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<UserDto>> GetRefreshTokenAsync(string refreshToken)
    {
        var user = await repository.GetByRefreshTokenAsync(refreshToken);
        if (user == null) return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.NotFound);

        if (!await tokenService.SetRefreshTokenAsync(user)) 
            return ServiceResultBuilder.BuildFailure<UserDto>(ServiceResultError.TokenError);
        
        var userDto = MapUserToDto(user);
        userDto.AccessToken = await tokenService.GenerateAccessToken(user);
        
        return ServiceResultBuilder.BuildSuccess(userDto);
    }


    //Helper functions
    private UserDto MapUserToDto(User user)
    {
        return mapper.Map<UserDto>(user);
    }
    
    private async Task<User?> GetUserFromRepositoryAsync(string id)
    {
        return await repository.GetByIdAsync(id);
    }

    private Worker MapRegisterToWorker(RegisterDto registerDto)
    {
        return mapper.Map<Worker>(registerDto);
    }
    
    private Employer MapRegisterToEmployer(RegisterDto registerDto)
    {
        return mapper.Map<Employer>(registerDto);
    }

    private async Task<bool> AddRolesToUser(List<string> roles, User user)
    {
        var result = await repository.AddRolesAsync(user, roles);
        return result.Succeeded;
    }

    private async Task<Worker?> RegisterWorker(RegisterDto registerDto)
    {
        var worker = MapRegisterToWorker(registerDto);
        worker.UserName = worker.Email?.ToLowerInvariant();
        
        var result = await repository.AddWorkerAsync(worker, registerDto.Password);
        
        if (!result.Succeeded) return null;

        if (!await AddRolesToUser(["Worker"], worker)) return null;
        return worker;
    }
    
    private async Task<Employer?> RegisterEmployer(RegisterDto registerDto)
    {
        var employer = MapRegisterToEmployer(registerDto);
        employer.UserName = employer.Email?.ToLowerInvariant();
        
        var result = await repository.AddEmployerAsync(employer, registerDto.Password);
        
        if (!result.Succeeded) return null;

        if (!await AddRolesToUser(["Employer"], employer)) return null;
        return employer;
    }

    private async Task<bool> SetRefreshTokenAsync(User user)
    {
        return await tokenService.SetRefreshTokenAsync(user);
    }

    private static string BuildFileName(string id, string extension)
    {
        return $"{id}{extension}";
    }

    private static string GetExtesion(string fileName)
    {
        return Path.GetExtension(fileName);
    }

    private async Task<string> UploadProfilePictureAndGetUrlAsync(IFormFile file, string fileName, string extension)
    {
        await using var stream = file.OpenReadStream();
        return await profilePictureService.UploadAsync(stream, fileName, extension);
    }

    private async Task<bool> UploadUrlToRepositoryAsync(string url, User user)
    {
        user.PictureUrl = url;
            
        var result = await repository.UpdateAsync(user);

        return result.Succeeded;
    }
}