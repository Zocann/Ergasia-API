using AutoMapper;
using Ergasia_API.DTOs.User;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserRepository repository, ITokenService tokenService, 
    IMapper mapper, IAuthorizationService authorizationService,
    IProfilePictureService profilePictureService) : ControllerBase
{
    [Authorize]
    [HttpGet("{id}")]
    public async Task<UserDto?> GetUser(string id)
    {
        if (!IsValidModelState()) return null;

        if (!await AuthorizeUser(id))
        {
            Response.StatusCode = 401;
            return null;
        }

        var user = await repository.GetByIdAsync(id);

        if (user == null)
        {
            Response.StatusCode = 404;
            return null;
        }
        
        return mapper.Map<UserDto>(user);
    }
    
    [Authorize]
    [HttpGet("role/{id}")]
    public async Task<string?> GetRole(string id)
    {
        if (!IsValidModelState()) return null;

        if (!await AuthorizeUser(id)) return null;

        var user = await repository.GetByIdAsync(id);

        if (user == null)
        {
            Response.StatusCode = 404;
            return null;
        }

        foreach (var role in await repository.GetRolesAsync(user))
        {
            return role;
        }

        Response.StatusCode = 404;
        return null;
    }
    
    [HttpGet("email/{email}")]
    public async Task<bool?> ValidateEmail(string email)
    {
        if (!IsValidModelState()) return null;

        var user = await repository.GetByEmailAsync(email);

        if (user != null) return false;
        
        Response.StatusCode = 404;
        return null;

    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<UserDto?> RegisterEmployerAsync(RegisterDto registerDto, [FromQuery] string userType)
    {
        if (!IsValidModelState()) return null;
        
        var result = await repository.RegisterAsync(registerDto, userType);

        if (!result.Succeeded)
        {
            Response.StatusCode = 400;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }

            return null;
        }
        
        var user = await repository.GetByEmailAsync(registerDto.Email);
        if (user == null) throw new ApplicationException("Unable to create user");
        
        if (!await tokenService.SetRefreshToken(user)) throw new ApplicationException("Unable to set refresh token");
        
        var userDto = mapper.Map<UserDto>(user);
        userDto.AccessToken = await tokenService.GenerateAccessToken(user);
        
        Response.StatusCode = 201;
        Response.Headers.Location = $"/Employers/{user.Id}";
        
        return userDto;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        if (!IsValidModelState()) return null;
        
        var user = await repository.LoginAsync(loginDto);

        if (user == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("identity", "Invalid credentials");
            return null;
        }

        if (!await tokenService.SetRefreshToken(user)) throw new ApplicationException("Unable to set refresh token");
        
        var userDto = mapper.Map<UserDto>(user);
        userDto.AccessToken = await tokenService.GenerateAccessToken(user);
        
        Response.Headers.Location = $"/Account/{user.Id}";
        return userDto;
    }
    
    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<UserDto?> UpdateAsync(string id, UpdateUserDto userDto)
    {
        if (!IsValidModelState()) return null;

        if (id != userDto.Id)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Id", "UserId does not match Id in route");
            return null;
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SameUserOrAdmin");
        if (!authorizationResult.Succeeded)
        {
            Response.StatusCode = 403;
            return null;
        }
        
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Account with provided Id does not exist");
            return null;
        }
        
        var result = await repository.UpdateAsync(mapper.Map(userDto, user));

        if (result == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Id", "Account with provided Id does not exist");
            return null;
        }

        if (! result.Succeeded)
        {
            Response.StatusCode = 400;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }
            return null;
        }
        
        return mapper.Map<UserDto>(user);
    }
    
    [Authorize]
    [HttpPost("{id}/picture")]
    public async Task<UserDto?> UploadPictureAsync(IFormFile file, string id)
    {
        if (!IsValidModelState()) return null;

        if (file.Length == 0)
        {
            Response.StatusCode = 400;
            return null;
        }

        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
        {
            Response.StatusCode = 415;
            return null;
        }
        
        var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SameUserOrAdmin");
        if (!authorizationResult.Succeeded)
        {
            Response.StatusCode = 403;
            return null;
        }

        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Account with provided Id does not exist");
            return null;
        }
        
        //Getting extension from file
        var extension = Path.GetExtension(file.FileName);

        if (string.IsNullOrEmpty(extension))
        {
            Response.StatusCode = 415;
            return null;
        }
        
        var fileName = $"{id}{extension}";
        
        await using (var stream = file.OpenReadStream())
        {
            var url = await profilePictureService.UploadAsync(stream, fileName, extension);

            user.PictureUrl = url;
            
            var result = await repository.UpdateAsync(user);

            if (result == null)
            {
                Response.StatusCode = 404;
                ModelState.AddModelError("Id", "Account with provided Id does not exist");
                return null;
            }

            if (! result.Succeeded)
            {
                Response.StatusCode = 400;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("identity", error.Description);
                }
                return null;
            }
        }

        return mapper.Map<UserDto>(user);
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        var result = await repository.DeleteAsync(id);

        if (result == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Id", "Account with provided Id does not exist");
            return;
        }
        
        if (!result.Succeeded)
        {
            Response.StatusCode = 400;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }
        }
    }
    
    [AllowAnonymous]
    [HttpGet("refresh-token")]
    public async Task<UserDto?> RefreshTokenAsync()
    {
        const string tokenName = "refreshToken";
        var headers = Request.Headers;
        var refreshToken = Request.Cookies[tokenName];
        
        if (refreshToken == null)
        {
            Response.StatusCode = 404;
            return null;
        }
        
        var user = await repository.GetByRefreshTokenAsync(refreshToken);

        if (user == null)
        {
            Response.StatusCode = 401;
            return null;
        }

        if (!await tokenService.SetRefreshToken(user)) throw new ApplicationException("Unable to set refresh token");
        
        var userDto = mapper.Map<UserDto>(user);
        userDto.AccessToken = await tokenService.GenerateAccessToken(user);
        
        return userDto;
    }
    
    //Helper functions

    private bool IsValidModelState()
    {
        if (ModelState.IsValid) return true;
        
        Response.StatusCode = 400;
        return false;
    }
    
    private async Task<bool> AuthorizeUser(string id)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SameUserOrAdmin");
        if (authorizationResult.Succeeded) return true;

        Response.StatusCode = 403;
        ModelState.AddModelError("Forbidden", "Account is not authorized to perform this action");
        return false;
    }
}