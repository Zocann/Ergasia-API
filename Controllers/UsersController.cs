using Ergasia_API.DTOs.User;
using Ergasia_API.Helpers;
using Ergasia_API.Services.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService, IAuthorizationService authorizationService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<UserDto?> GetUserAsync(string id)
    {
        if (! ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (! await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await userService.GetAsync(id);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [Authorize]
    [HttpGet("role/{id}")]
    public async Task<string?> GetRole(string id)
    {
        if (! ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (! await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await userService.GetSingleRoleAsync(id);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpGet("email/{email}")]
    public async Task<bool?> ValidateEmail(string email)
    {
        if (! ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }

        var serviceResult = await userService.UserWithThisEmailExists(email);
        return !serviceResult.Data;

    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<UserDto?> RegisterAsync([FromBody] RegisterDto registerDto, [FromQuery] string userType)
    {
        if (! ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }

        var serviceResult = await userService.RegisterAsync(registerDto, userType);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        if (! ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }

        var serviceResult = await userService.LoginAsync(loginDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [Authorize]
    [HttpPatch("update/{id}")]
    public async Task<UserDto?> PatchAsync(string id, UpdateUserDto updateUserDto)
    {
        if (! ModelState.IsValid || ! IdsMatch(id, updateUserDto.Id))
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (! await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await userService.UpdateAsync(updateUserDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [Authorize]
    [HttpPost("{id}/picture")]
    public async Task<bool> UploadPictureAsync(IFormFile file, string id)
    {
        if (! ModelState.IsValid || file.Length < 1)
        {
            SetStatusCodeTo(400);
            return false;
        }
        if (! await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return false;
        }
        if (IsInvalidFileFormat(file))
        {
            Response.StatusCode = 415;
            return false;
        }

        var serviceResult = await userService.UploadPictureAsync(file, id);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        if (! ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return;
        }
        //Only admin can delete user
        if (! await IsSameUserOrAdminAsync(string.Empty))
        {
            SetStatusCodeTo(401);
            return;
        }

        var serviceResult = await userService.DeleteAsync(id);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
    }
    
    [AllowAnonymous]
    [HttpGet("refresh-token")]
    public async Task<UserDto?> RefreshTokenAsync()
    {
        const string tokenName = "refreshToken";
        var refreshToken = GetRefreshTokenFromCookie(tokenName);
        
        if (refreshToken == null)
        {
            SetStatusCodeTo(404);
            return null;
        }
        
        var serviceResult = await userService.GetRefreshTokenAsync(refreshToken);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    
    
    //Helper functions
    private static bool IdsMatch(string firstId, string secondId)
    {
        return firstId == secondId;
    }

    private async Task<bool> IsSameUserOrAdminAsync(string id)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SameUserOrAdmin");
        return authorizationResult.Succeeded;
    }
    
    private void SetStatusCodeTo(int statusCode) => Response.StatusCode = statusCode;

    private static bool IsInvalidFileFormat(IFormFile file)
    {
        return file.ContentType != "image/jpeg" && file.ContentType != "image/png";
    }

    private string? GetRefreshTokenFromCookie(string refreshTokenName)
    {
        return Request.Cookies[refreshTokenName];
    }
}