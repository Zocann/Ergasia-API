using Ergasia_API.DTOs.Job;
using Ergasia_API.Helpers;
using Ergasia_API.Services.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("Employers/{employerId}/[controller]")]
public class JobsController(
    IJobService jobService,
    IWorkerJobService workerJobService,
    IWorkerJobRequestService workerJobRequestService,
    IAuthorizationService authorizationService) : ControllerBase
{
    public record MessageDto
    {
        public readonly string? Message = null;
    }

    [HttpGet("/Jobs")]
    public async Task<IEnumerable<JobDto>?> GetAllUpcomingAsync()
    {
        var serviceResult = await jobService.GetAllUpcomingAsync();
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<JobDto>?> GetAllFromEmployerAsync(string employerId)
    {
        var serviceResult = await jobService.GetAllFromEmployerAsync(employerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<JobDto?> GetAsync(string id)
    {
        var serviceResult = await jobService.GetAsync(id);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpGet("{id}/Work-spots")]
    [Authorize]
    public async Task<int?> GetWorkSpotsAsync(string id)
    {
        var serviceResult = await jobService.GetJobWorkSpots(id);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpPost]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<JobDto?> PostAsync(string employerId, JobDto jobDto)
    {
        if (! ModelState.IsValid || ! IdsMatch(employerId, jobDto.EmployerId))
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (! await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(401);
            return null;
        }
        
        var serviceResult = await jobService.AddAsync(jobDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpPatch("{jobId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<JobDto?> UpdateAsync(JobDto jobDto, string jobId, string employerId)
    {
        if (! ModelState.IsValid || ! IdsMatch(jobId, jobDto.Id) || ! IdsMatch(employerId, jobDto.EmployerId))
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (!await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(401);
            return null;
        }
        
        var serviceResult = await jobService.UpdateAsync(jobDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task DeleteAsync(string id)
    {
        if (!await IsSameUserOrAdminAsync(id))
        {
            SetStatusCodeTo(401);
            return;
        }
        
        var serviceResult = await jobService.DeleteAsync(id);

        SetStatusCodeTo(serviceResult.IsSuccess ? 
            204 : 
            GetStatusCode.BasedOnError(serviceResult.Error));
    }


    [Authorize(Roles = "Employer,Admin")]
    [HttpGet("{jobId}/Requests")]
    public async Task<IEnumerable<JobRequestDto>?> GetJobRequestsAsync(string employerId, string jobId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (!await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await workerJobRequestService.GetAllFromEmployerAsync(employerId, jobId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [Authorize]
    [HttpGet("/Workers/{workerId}/Jobs/Requests")]
    public async Task<IEnumerable<JobRequestDto>?> GetJobRequestsByWorkerIdAsync(string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (!await IsSameUserOrAdminAsync(workerId))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await workerJobRequestService.GetAllFromWorkerAsync(workerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [Authorize]
    [HttpGet("{jobId}/Requests/{workerId}")]
    public async Task<JobRequestDto?> GetJobRequestAsync(string workerId, string jobId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (!await IsSameUserOrAdminAsync(workerId))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await workerJobRequestService.GetAsync(workerId, jobId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    
    [HttpPost("{jobId}/Requests/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<JobRequestDto?> PostJobRequestAsync(string jobId, string workerId, [FromBody] MessageDto message)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (!await IsSameUserOrAdminAsync(workerId))
        {
            SetStatusCodeTo(401);
            return null;
        }
        
        var serviceResult = await workerJobRequestService.AddAsync(workerId, jobId, message.Message);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [Authorize(Roles = "Employer,Admin")]
    [HttpDelete("{jobId}/Requests/{workerId}")]
    public async Task DeleteJobRequestAsync(string jobId, string employerId, string workerId)

    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return;
        }
        if (!await IsSameUserOrAdminAsync(workerId) || !await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(401);
            return;
        }
        
        var serviceResult = await workerJobRequestService.DeleteAsync(workerId, jobId);
        SetStatusCodeTo(serviceResult.IsSuccess ? 204 : GetStatusCode.BasedOnError(serviceResult.Error));
    }


    [HttpGet("{jobId}/Workers")]
    [Authorize]
    public async Task<IEnumerable<WorkerJobDto>?> GetWorkerJobsByJobIdAsync(string jobId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }

        var serviceResult = await workerJobService.GetAllByJobIdAsync(jobId);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpGet("/Workers/{workerId}/Jobs")]
    [Authorize]
    public async Task<IEnumerable<WorkerJobDto>?> GetWorkerJobsByWorkerIdAsync(string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }

        var serviceResult = await workerJobService.GetAllByWorkerIdAsync(workerId);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpGet("{jobId}/Workers/{workerId}")]
    [Authorize]
    public async Task<WorkerJobDto?> GetWorkerJobAsync(string jobId, string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }

        var serviceResult = await workerJobService.GetAsync(workerId, jobId);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }
    
    [HttpPost("{jobId}/Workers/{workerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<WorkerJobDto?> PostWorkerJobAsync(string employerId, string jobId, string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return null;
        }
        if (! await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(401);
            return null;
        }

        var serviceResult = await workerJobService.AddAsync(workerId, jobId);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpDelete("{jobId}/Workers/{workerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task DeleteWorkerJobAsync(string employerId, string jobId, string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return;
        }
        if (! await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(403);
            return;
        }

        var serviceResult = await workerJobService.DeleteRatingAsync(workerId, jobId);

        SetStatusCodeTo(serviceResult.IsSuccess ? 
            204 : 
            GetStatusCode.BasedOnError(serviceResult.Error));
    }
    
    
    //Helper functions
    private async Task<bool> IsSameUserOrAdminAsync(string id)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "SameUserOrAdmin");
        return authorizationResult.Succeeded;
    }
    
    private static bool IdsMatch(string? firstId, string? secondId)
    {
        return firstId == secondId;
    }
    
    private void SetStatusCodeTo(int statusCode) => Response.StatusCode = statusCode;
}