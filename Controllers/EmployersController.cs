using AutoMapper;
using Ergasia_API.DTOs.Employer;
using Ergasia_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployersController(IEmployerRepository repository, IMapper mapper, 
    IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet]
    public async Task<List<EmployerDto>> GetAllAsync()
    {
        var employers = await repository.GetAllAsync();
        List<EmployerDto> result = [];
        if (employers.Count > 0)
        {
            result.AddRange(employers.Select(mapper.Map<EmployerDto>));
        }
        return result;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<EmployerDto?> GetAsync(string id)
    {
        var employer = await repository.GetByIdAsync(id);
        
        if (employer != null) return mapper.Map<EmployerDto>(employer);
        
        ModelState.AddModelError("Id", "Employer with provided Id does not exist");
        Response.StatusCode = 404;
        return null;
    }
    
    [Authorize(Roles = "Employer,Admin")]
    [HttpPatch("{id}")]
    public async Task<EmployerDto?> PatchAsync(string id, EmployerDto employerDto)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return null;
        }
        
        if (id != employerDto.Id)
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
        
        var employer = await repository.GetByIdAsync(id);

        if (employer == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Employer does not exist");
            return null;
        }
        
        employerDto.IsActive = employer.IsActive;
        employerDto.RefreshToken = employer.RefreshToken;
        employerDto.RefreshTokenExpiration = employer.RefreshTokenExpiration;
        employerDto.PictureUrl = employer.PictureUrl;
        employerDto.DateOfRegistration = employer.DateOfRegistration;
        
        employer = await repository.UpdateAsync(mapper.Map(employerDto, employer));
        return mapper.Map<EmployerDto>(employer);
    }
}