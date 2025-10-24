using AutoMapper;
using Ergasia_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTOs_WorkerDto = Ergasia_API.DTOs.Worker.WorkerDto;
using WorkerDto = Ergasia_API.DTOs.Worker.WorkerDto;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkersController(IWorkerRepository repository, IMapper mapper, 
    IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<List<WorkerDto>> GetAllAsync()
    {
        var workers = await repository.GetAllAsync();
        List<WorkerDto> result = [];
        if (workers.Count > 0)
        {
            result.AddRange(workers.Select(mapper.Map<WorkerDto>));
        }
        return result;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<DTOs_WorkerDto?> GetAsync(string id)
    {
        var worker = await repository.GetByIdAsync(id);

        if (worker != null) return mapper.Map<DTOs_WorkerDto>(worker);
        
        ModelState.AddModelError("Id", "Worker with provided Id does not exist");
        Response.StatusCode = 404;
        return null;
    }
    
    [HttpPatch("{id}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<WorkerDto?> PatchAsync(string id, WorkerDto workerDto)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return null;
        }
        
        if (id != workerDto.Id)
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
        
        var worker = await repository.GetByIdAsync(id);

        if (worker == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Worker does not exist");
            return null;
        }
        
        workerDto.RefreshToken = worker.RefreshToken;
        workerDto.RefreshTokenExpiration = worker.RefreshTokenExpiration;
        workerDto.PictureUrl = worker.PictureUrl;
        workerDto.IsActive = worker.IsActive;
        workerDto.DateOfRegistration = worker.DateOfRegistration;
        
        worker = await repository.UpdateAsync(mapper.Map(workerDto, worker));
        return mapper.Map<WorkerDto>(worker);
    }
}