using AutoMapper;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Employer;
using Ergasia_API.Helpers;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployersController(IEmployerService employerService, IMapper mapper, 
    IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<EmployerDto>?> GetAllAsync()
    {
        var serviceResult = await employerService.GetAllAsync();
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<EmployerDto?> GetAsync(string id)
    {
        var serviceResult = await employerService.GetByIdAsync(id);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [Authorize(Roles = "Employer,Admin")]
    [HttpPatch("{id}")]
    public async Task<EmployerDto?> PatchAsync(string id, EmployerDto employerDto)
    {
        if (! ModelState.IsValid || ! IdsMatch(id, employerDto.Id))
        {
            SetStatusCodeTo(400);
            return null;
        }

        if (!await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return null;
        }
        
        var serviceResult = await employerService.UpdateAsync(employerDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
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
}