using AutoMapper;
using Ergasia_API.DTOs.Job;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("Employers/{employerId}/[controller]")]
public class JobsController(
    IJobRepository repository,
    IEmployerRepository employerRepository,
    IWorkerJobRepository workerJobRepository,
    IWorkerJobRequestRepository workerJobRequestRepository,
    IMapper mapper,
    IAuthorizationService authorizationService) : ControllerBase
{
    public record MessageDto
    {
        public string? Message { get; }    
    }

    [HttpGet("/Jobs")]
    public async Task<IEnumerable<JobDto?>> GetAllUpcomingAsync()
    {
        var result = new List<JobDto>();
        var jobs = repository.GetAllAsync();
        
        await foreach (var job in jobs)
        {
            if (job?.DateOfBegin > DateTime.UtcNow) result.Add(mapper.Map<JobDto>(job));
        }
        
        return result;
    }

    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<JobDto?>> GetAllFromEmployerAsync(string employerId)
    {
        var result = new List<JobDto>();
        var jobs = repository.GetByEmployerIdAsync(employerId);
        await foreach (var job in jobs)
        {
            result.Add(mapper.Map<JobDto>(job));
        }
        
        return result;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<JobDto?> GetAsync(string id)
    {
        var job = await repository.GetByIdAsync(id);

        if (job != null) return mapper.Map<JobDto>(job);

        ModelState.AddModelError("Id", "Job with provided id does not exist");
        Response.StatusCode = 404;
        return null;
    }
    
    [HttpGet("{id}/Work-spots")]
    [Authorize]
    public async Task<int?> GetWorkSpotsAsync(string id)
    {
        var spots = await repository.AvailableWorkSpots(id);

        if (spots == null) Response.StatusCode = 404;

        return spots;
    }

    [HttpPost]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<JobDto?> PostAsync(string employerId, JobDto jobDto)
    {
        if (!IsValidModelState()) return null;
        
        if (jobDto.EmployerId != employerId)
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Id", "Employer id does not match employer in route");
            return null;
        }
        
        if (! await AuthorizeUser(employerId)) return null;
        
        var employer = await employerRepository.GetByIdAsync(employerId);

        if (employer == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Employer", "Employer does not exist");
            return null;
        }

        jobDto.Id = Guid.NewGuid().ToString();

        var job = mapper.Map<Job>(jobDto);
        job.Employer = employer;
        
        await repository.AddAsync(job);
        
        Response.StatusCode = 201;
        Response.Headers.Location = $"/Employers/{employerId}/Jobs/{job.Id}";
        return mapper.Map<JobDto>(job);
    }

    [HttpPatch("{jobId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<JobDto?> UpdateAsync(JobDto jobDto, string jobId, string employerId)
    {
        if (!IsValidModelState()) return null;
        
        if (jobDto.Id == null)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Id", "Id cannot be null");
            return null;
        }

        if (jobId != jobDto.Id)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Id", "Job id does not match id route");
            return null;
        }
        
        //Checking if job is not current or not ended
        var job = await repository.GetByIdAsync(jobId);

        if (job == null)
        {
            Response.StatusCode = 404;
            return null;
        }

        if (job.EmployerId != jobDto.EmployerId || jobDto.EmployerId != employerId)
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Record", "This job does not belong to this employer");
            return null;
        }
        
        if (! await AuthorizeUser(employerId)) return null;

        if (job.DateOfBegin <= DateTime.Now && !User.IsInRole("Admin"))
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Record", "Cannot update progressing or finished job");
            return null;
        }
        
        var updatedJob = await repository.UpdateAsync(mapper.Map<Job>(jobDto));

        if (updatedJob == null)
        {
            Response.StatusCode = 404;
            return null;
        }
        
        return mapper.Map<JobDto>(updatedJob);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task Delete(string id)
    {
        var deleted = await repository.DeleteAsync(id);

        Response.StatusCode = deleted ? 200 : 404;
    }


    [Authorize(Roles = "Employer,Admin")]
    [HttpGet("{jobId}/Requests")]
    public async Task<IEnumerable<JobRequestDto?>> GetJobRequestsAsync(string employerId, string jobId)
    {
        var result = new List<JobRequestDto>();
        
        if (!IsValidModelState()) return result;
        if (! await AuthorizeUser(employerId)) return result;

        var jobRequests = workerJobRequestRepository.GetByEmployerIdAsync(employerId, jobId);

        await foreach (var jobRequest in jobRequests)
        {
            result.Add(mapper.Map<JobRequestDto>(jobRequest));
        }
        
        return result;
    }
    
    [Authorize]
    [HttpGet("/Workers/{workerId}/Jobs/Requests")]
    public async Task<IEnumerable<JobRequestDto?>?> GetJobRequestsByWorkerIdAsync(string workerId)
    {
        var result = new List<JobRequestDto>();
        
        if (!IsValidModelState()) return result;
        if (! await AuthorizeUser(workerId)) return result;

        var jobRequests = workerJobRequestRepository.GetByWorkerId(workerId);

        await foreach (var jobRequest in jobRequests)
        {
            result.Add(mapper.Map<JobRequestDto>(jobRequest));
        }
        
        return result;
    }
    
    [Authorize]
    [HttpGet("{jobId}/Requests/{workerId}")]
    public async Task<JobRequestDto?> GetJobRequestAsync(string workerId, string jobId)
    {
        if (!IsValidModelState()) return null;
        
        if (! await AuthorizeUser(workerId)) return null;

        var jobRequest = await workerJobRequestRepository.GetAsync(workerId, jobId);
        if (jobRequest == null)
        {
            Response.StatusCode = 404;
            return null;
        }
        
        return mapper.Map<JobRequestDto>(jobRequest);
    }
    
    
    [HttpPost("{jobId}/Requests/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<JobRequestDto?> PostJobRequestAsync(string jobId, string workerId, [FromBody] MessageDto message)
    {
        if (!IsValidModelState()) return null;
        
        if (! await AuthorizeUser(workerId)) return null;

        var job = await repository.GetByIdAsync(jobId);

        if (job == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Job does not exist");
            return null;
        }
        
        //Check if job didnt already finished
        if (job.DateOfBegin.AddDays(job.Duration) < DateTime.UtcNow)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Date of begin", "Job has already finished");
            return null;
            
        }
        
        //Check if there already isn't job request between this job and worker
        var workerJobRequest = await workerJobRequestRepository.GetAsync(workerId, jobId);

        if (workerJobRequest != null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Request already exists");
            return null;
        }
        
        workerJobRequest = await workerJobRequestRepository.AddAsync(workerId, jobId, message.Message);
        
        if (workerJobRequest == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Id", "Worker or job does not exist");
        }
        
        return mapper.Map<JobRequestDto>(workerJobRequest);
    }

    [Authorize(Roles = "Employer,Admin")]
    [HttpDelete("{jobId}/Requests/{workerId}")]
    public async Task DeleteJobRequestAsync(string jobId, string employerId, string workerId)

    {
        if (!IsValidModelState()) return;
        
        if (User.IsInRole("Employer")) if (! await AuthorizeUser(employerId)) return;
        if (User.IsInRole("Worker")) if (! await AuthorizeUser(workerId)) return;

        var jobRequest = await workerJobRequestRepository.GetAsync(workerId, jobId);

        if (jobRequest == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Request does not exist");
            return;
        }

        if (jobRequest.WorkerId != workerId || jobRequest.JobId != jobId || jobRequest.Job.EmployerId != employerId)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Id", "Provided id's do not match request id's");
            return;
        }

        Response.StatusCode = await workerJobRequestRepository.DeleteAsync(workerId, jobId) 
            ? 204 : 400;
    }


    [HttpGet("{jobId}/Workers")]
    [Authorize]
    public async Task<IEnumerable<WorkerJobDto?>> GetWorkerJobsAsync(string jobId)
    {
        var result = new List<WorkerJobDto>();
        if (!IsValidModelState()) return result;
        
        var workerJobs = workerJobRepository.GetByJobIdAsync(jobId);

        await foreach (var workerJob in workerJobs)
        {
            result.Add(mapper.Map<WorkerJobDto>(workerJob));
        }
        
        return result;
    }
    
    [HttpGet("/Workers/{workerId}/Jobs")]
    [Authorize]
    public async Task<IEnumerable<WorkerJobDto?>> GetWorkerJobsByWorkerIdAsync(string workerId)
    {
        var result = new List<WorkerJobDto>();
        if (!IsValidModelState()) return result;
        
        var workerJobs = workerJobRepository.GetByWorkerIdAsync(workerId);

        await foreach (var workerJob in workerJobs)
        {
            result.Add(mapper.Map<WorkerJobDto>(workerJob));
        }
        
        return result;
    }
    
    [HttpGet("{jobId}/Workers/{workerId}")]
    [Authorize]
    public async Task<WorkerJobDto?> GetWorkerJobAsync(string jobId, string workerId)
    {
        if (!IsValidModelState()) return null;
        
        var workerJob = await workerJobRepository.GetAsync(workerId, jobId);

        if (workerJob == null)
        {
            Response.StatusCode = 404;
            return null;
        }
        
        return mapper.Map<WorkerJobDto>(workerJob);
    }
    
    [HttpPost("{jobId}/Workers/{workerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<WorkerJobDto?> PostWorkerJobAsync(string employerId, string jobId, string workerId)
    {
        if (!IsValidModelState()) return null;
        
        if (! await AuthorizeUser(employerId)) return null;

        var jobRequest = await workerJobRequestRepository.GetAsync(workerId, jobId);

        if (jobRequest == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "Request does not exist");
            return null;
        }

        if (jobRequest.WorkerId != workerId || jobRequest.JobId != jobId || jobRequest.Job.EmployerId != employerId)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Id", "Provided id's do not match request id's");
            return null;
        }
        
        //Delete Job request
        await workerJobRequestRepository.DeleteAsync(workerId, jobId);
        
        //Add WorkerJob record
        var workerJob = await workerJobRepository.AddAsync(workerId, jobId);
        
        if (workerJob == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Id", "Worker or job does not exist");
        }
        
        return mapper.Map<WorkerJobDto>(workerJob);
    }

    [HttpDelete("{jobId}/Workers/{workerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task DeleteWorkerJobAsync(string employerId, string jobId, string workerId)
    {
        if (!IsValidModelState()) return;

        if (!await AuthorizeUser(employerId)) return;

        var workerJob = await workerJobRepository.GetAsync(workerId, jobId);

        if (workerJob == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "WorkerJob does not exist");
            return;
        }

        if (workerJob.WorkerId != workerId || workerJob.JobId != jobId || workerJob.Job.EmployerId != employerId)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Id", "Provided id's do not match request id's");
            return;
        }
        
        //Check if the WorkerJob isn't in progress or already finished
        if (workerJob.Job.DateOfBegin <= DateTime.Now && !User.IsInRole("Admin"))
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Record", "Cannot delete progressing or finished worker and job record");
            return;
        }

        if (!await workerJobRepository.DeleteAsync(workerId, jobId))
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "WorkerJob does not exist");
        }
        
        Response.StatusCode = 204;
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