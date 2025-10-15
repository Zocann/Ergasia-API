using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Ergasia_API.DTOs.Job;
using Ergasia_API.DTOs.Rating;
using Ergasia_API.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingsController(
    IJobRepository jobRepository,
    IEmployerRatingRepository employerRatingRepository,
    IWorkerRatingRepository workerRatingRepository,
    IWorkerJobRepository workerJobRepository,
    IMapper mapper,
    IAuthorizationService authorizationService) : ControllerBase
{
    public record VerbalRatingDto
    {
        public string? VerbalRating { get; set; }    
    }
    
    //Get all ratings

    [HttpGet("/Employers/{employerId}/Ratings")]
    [AllowAnonymous]
    public async Task<IEnumerable<EmployerRatingDto?>> GetEmployerRatingsAsync(string employerId)
    {
        var result = new List<EmployerRatingDto>();
        var employerRatings = employerRatingRepository.GetAllByIdAsync(employerId);

        await foreach (var employerRating in employerRatings)
        {
            result.Add(mapper.Map<EmployerRatingDto>(employerRating));
        }
        return result;
    }

    [HttpGet("/Workers/{workerId}/Ratings")]
    [AllowAnonymous]
    public async Task<IEnumerable<WorkerRatingDto?>> GetWorkerRatingsAsync(string workerId)
    {
        var result = new List<WorkerRatingDto>();
        var workerRatings = workerRatingRepository.GetAllByIdAsync(workerId);

        await foreach (var workerRating in workerRatings)
        {
            result.Add(mapper.Map<WorkerRatingDto>(workerRating));
        }
        return result;
    }

    [HttpGet("/Employers/{employerId}/Jobs/{jobId}/Ratings")]
    [AllowAnonymous]
    public async Task<IEnumerable<WorkerJobDto?>> GetJobRatingsAsync(string jobId)
    {
        var result = new List<WorkerJobDto>();
        var workerJobs = workerJobRepository.GetByJobIdAsync(jobId);

        //Filtering WorkerJobs through numerical rating and returning dto
        await foreach (var workerJob in workerJobs)
        {
            if (workerJob?.NumericalRating != null) result.Add(mapper.Map<WorkerJobDto>(workerJob));
        }
        return result;
    }
    
    
    
    

    //Get single rating
    [HttpGet("/Employers/{employerId}/Ratings/{workerId}")]
    [AllowAnonymous]
    public async Task<EmployerRatingDto?> GetEmployerRatingAsync(string employerId, string workerId)
    {
        var employerRating = await employerRatingRepository.GetAsync(employerId, workerId);

        if (employerRating == null)
        {
            Response.StatusCode = 404;
            return null;
        }

        if (employerRating.EmployerId == employerId) return mapper.Map<EmployerRatingDto>(employerRating);

        Response.StatusCode = 400;
        ModelState.AddModelError("Employer id", "Employer id in route does not match employer id in rating record");
        return null;
    }

    [HttpGet("/Workers/{workerId}/Ratings/{employerId}")]
    [AllowAnonymous]
    public async Task<WorkerRatingDto?> GetWorkerRatingAsync(string workerId, string employerId)
    {
        var workerRating = await workerRatingRepository.GetAsync(workerId, employerId);

        if (workerRating == null)
        {
            Response.StatusCode = 404;
            return null;
        }

        if (workerRating.WorkerId == workerId) return mapper.Map<WorkerRatingDto>(workerRating);

        Response.StatusCode = 400;
        ModelState.AddModelError("Worker id", "Worker id in route does not match worker id in rating record");
        return null;
    }

    [HttpGet("/Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}")]
    [AllowAnonymous]
    public async Task<WorkerJobDto?> GetJobRatingAsync(string employerId, string jobId, string workerId)
    {
        var job = await jobRepository.GetByIdAsync(jobId);

        if (job == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Job", "Job not found");
            return null;
        }

        if (job.EmployerId != employerId)
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Record", "Job does not belong to this employer");
            return null;
        }

        var workerJob = await workerJobRepository.GetAsync(workerId, jobId);

        if (workerJob == null)
        {
            Response.StatusCode = 404;
            return null;
        }

        if (workerJob.JobId != jobId)
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Record", "Rating does not belong to this job");
            return null;
        }

        if (workerJob.NumericalRating == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "Rating does not exist");
            return null;
        }
        
        return mapper.Map<WorkerJobDto>(workerJob);
    }
    
    
    
    
    
    
    
    
    //Average ratings
    [HttpGet("/Employers/{employerId}/Average-rating")]
    [AllowAnonymous]
    public async Task<decimal?> GetEmployerAverageRatingAsync(string employerId)
    {
        var result = new List<EmployerRatingDto>();
        var employerRatings = employerRatingRepository.GetAllByIdAsync(employerId);

        await foreach (var employerRating in employerRatings)
        {
            result.Add(mapper.Map<EmployerRatingDto>(employerRating));
        }
        
        if (result.Count == 0) return null;

        return (decimal?)result.Average(r => r.NumericalRating);

    }
    
    [HttpGet("/Workers/{workerId}/Average-rating")]
    [AllowAnonymous]
    public async Task<decimal?> GetWorkerAverageRatingAsync(string workerId)
    {
        var result = new List<WorkerRatingDto>();
        var workerRatings = workerRatingRepository.GetAllByIdAsync(workerId);

        await foreach (var workerRating in workerRatings)
        {
            result.Add(mapper.Map<WorkerRatingDto>(workerRating));
        }
        
        if (result.Count == 0) return null;

        return (decimal?)result.Average(r => r.NumericalRating);
    }
    
    [HttpGet("/Employers/{employerId}/Jobs/{jobId}/AverageRating")]
    [AllowAnonymous]
    public async Task<decimal?> GetJobAverageRatingAsync(string jobId)
    {
        var result = new List<WorkerJobDto>();
        var workerJobs = workerJobRepository.GetByJobIdAsync(jobId);

        //Filtering WorkerJobs through numerical rating and returning dto
        await foreach (var workerJob in workerJobs)
        {
            if (workerJob?.NumericalRating != null) result.Add(mapper.Map<WorkerJobDto>(workerJob));
        }
        
        if (result.Count == 0) return null;

        return (decimal?)result.Average(r => r.NumericalRating);
    }
    
    
    
    
    
    
    
    
    //Post ratings

    [HttpPost("/Employers/{employerId}/Ratings")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<EmployerRatingDto?> PostEmployerRatingAsync(string employerId,
        [Required] [FromQuery] string workerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
    {
        if (!IsValidModelState()) return null;

        if (!await AuthorizeUser(workerId)) return null;

        //Check if the worker did work for this employer
        if (!await IsEmployersPastWorker(employerId, workerId))
            return null;

        if (await employerRatingRepository.GetAsync(employerId, workerId) != null)
        {
            Response.StatusCode = 400;
            ModelState.AddModelError("Record", "Rating already exists");
        }

        var newEmployerRating =
            await employerRatingRepository.AddAsync(employerId, workerId, numericalRating, verbalRating?.VerbalRating);

        if (newEmployerRating == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Id", "Employer or worker with provided id does not exist");
            return null;
        }

        Response.StatusCode = 201;

        Response.Headers.Location = $"/Employers/{employerId}/Ratings/{workerId}";
        return mapper.Map<EmployerRatingDto>(newEmployerRating);
    }

    [HttpPost("/Workers/{workerId}/Ratings")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<WorkerRatingDto?> PostWorkerRatingAsync(string workerId,
        [Required] [FromQuery] string employerId,
        [Required] [FromQuery] [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
    {
        if (!IsValidModelState()) return null;

        if (!await AuthorizeUser(employerId)) return null;

        //Check if the employer di employ this worker
        if (!await IsEmployersPastWorker(employerId, workerId))
            return null;

        var workerRating =
            await workerRatingRepository.AddAsync(workerId, employerId, numericalRating, verbalRating?.VerbalRating);

        if (workerRating == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Id", "There is no record between provided worker id and job id");
            return null;
        }

        Response.StatusCode = 201;

        Response.Headers.Location = $"/Workers/{workerId}/Ratings/{employerId}";
        return mapper.Map<WorkerRatingDto>(workerRating);
    }
    
    
    
    

    [HttpPatch("/Employers/{employerId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<EmployerRatingDto?> PatchEmployerRating(string employerId, string workerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
    {
        if (!IsValidModelState()) return null;

        if (!await AuthorizeUser(workerId)) return null;

        //Check if the worker did work for this employer
        if (!await IsEmployersPastWorker(employerId, workerId))
            return null;

        var employerRating = await employerRatingRepository.UpdateAsync(employerId, workerId, numericalRating, verbalRating?.VerbalRating);

        if (employerRating == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "Rating does not exist");
            return null;
        }

        return mapper.Map<EmployerRatingDto>(employerRating);
    }

    [HttpPatch("/Workers/{workerId}/Ratings/{employerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<WorkerRatingDto?> PatchWorkerRating(string workerId, string employerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
    {
        if (!IsValidModelState()) return null;

        if (!await AuthorizeUser(employerId)) return null;

        //Check if the employer did employ for this worker
        if (!await IsEmployersPastWorker(employerId, workerId))
            return null;

        var workerRating = await workerRatingRepository.UpdateAsync(workerId, employerId, numericalRating, verbalRating?.VerbalRating);

        if (workerRating == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "Rating does not exist");
            return null;
        }

        return mapper.Map<WorkerRatingDto>(workerRating);
    }
    
    [HttpPatch("/Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<WorkerJobDto?> PatchJobRating(string employerId, string jobId, string workerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
    {
        if (!IsValidModelState()) return null;
        if (!await AuthorizeUser(workerId)) return null;
        if (!await ValidateWorkerJob(workerId, employerId, jobId)) return null;

        var workerJob = await workerJobRepository.UpdateRatingAsync(workerId, jobId, numericalRating, verbalRating?.VerbalRating);

        if (workerJob == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "There is no record between provided worker and job");
            return null;
        }

        Response.StatusCode = 201;
        return mapper.Map<WorkerJobDto>(workerJob);
    }
    
    
    
    

    [HttpDelete("/Employers/{employerId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task DeleteEmployerRating(string employerId, string workerId)
    {
        if (!IsValidModelState()) return;

        if (!await AuthorizeUser(workerId)) return;

        //Check if the worker did work for this employer
        if (!await IsEmployersPastWorker(employerId, workerId)) return;
        
        if (! await employerRatingRepository.DeleteAsync(employerId, workerId))
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "Rating does not exist");
            return;
        }
        
        Response.StatusCode = 204;
    }
    
    [HttpDelete("/Workers/{workerId}/Ratings/{employerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task DeleteWorkerRating(string workerId, string employerId)
    {
        if (!IsValidModelState()) return;

        if (!await AuthorizeUser(employerId)) return;

        //Check if the employer did employ this worker
        if (!await IsEmployersPastWorker(employerId, workerId)) return;
        
        if (! await workerRatingRepository.DeleteAsync(workerId, employerId))
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "Rating does not exist");
            return;
        }
        
        Response.StatusCode = 204;
    }

    [HttpDelete("/Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task DeleteJobRating(string employerId, string jobId, string workerId)
    {
        if (!IsValidModelState()) return;

        if (!await AuthorizeUser(workerId)) return;
        
        if (!await ValidateWorkerJob(workerId, employerId, jobId)) return;
        
        if (! await workerJobRepository.DeleteRatingAsync(workerId, jobId))
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Rating", "Rating does not exist");
            return;
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

    private async Task<bool> IsEmployersPastWorker(string employerId, string workerId)
    {
        var jobs = jobRepository.GetByEmployerIdAsync(employerId);

        await foreach (var job in jobs)
        {
            if (job == null) continue;
            if (await workerJobRepository.GetAsync(workerId, job.Id) != null) return true;
        }

        Response.StatusCode = 403;
        ModelState.AddModelError("Record", "There is no job record between provided employer and worker");
        return false;
    }

    private async Task<bool> ValidateWorkerJob(string workerId, string employerId, string jobId)
    {
        var workerJob = await workerJobRepository.GetAsync(workerId, jobId);

        if (workerJob == null)
        {
            Response.StatusCode = 404;
            ModelState.AddModelError("Record", "There is no record between provided worker and job");
            return false;
        }

        if (workerJob.Job.EmployerId != employerId)
        {
            Response.StatusCode = 403;
            ModelState.AddModelError("Record", "There is no record between provided employer and worker");
            return false;
        }

        return true;
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