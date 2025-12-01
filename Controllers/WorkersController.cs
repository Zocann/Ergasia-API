using AutoMapper;
using Azure;
using Ergasia_API.Data.Enums;
using Ergasia_API.Helpers;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs_WorkerDto = Ergasia_API.DTOs.Worker.WorkerDto;
using WorkerDto = Ergasia_API.DTOs.Worker.WorkerDto;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkersController(IWorkerService workerService, IMapper mapper, 
    IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<IEnumerable<WorkerDto>?> GetAllAsync()
    {
        var serviceResult = await workerService.GetAllAsync();
        
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        
        return serviceResult.Data;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<DTOs_WorkerDto?> GetAsync(string id)
    { 
        var serviceResult = await workerService.GetByIdAsync(id);
       if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
       return serviceResult.Data;
    }
    
    [HttpPatch("{id}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<WorkerDto?> PatchAsync(string id, WorkerDto workerDto)
    {
        if (! ModelState.IsValid || ! IdsMatch(id, workerDto.Id))
        {
            SetStatusCodeTo(400);
            return null;
        }

        if (!await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return null;
        }
        
        var serviceResult = await workerService.Update(workerDto);
        
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