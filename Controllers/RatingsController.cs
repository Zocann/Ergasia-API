using System.ComponentModel.DataAnnotations;
using Ergasia_API.Data;
using Ergasia_API.DTOs.Job;
using Ergasia_API.DTOs.Rating;
using Ergasia_API.Helpers;
using Ergasia_API.Services.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ergasia_API.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingsController(
    IWorkerJobService workerJobService,
    IEmployerRatingService employerRatingService,
    IWorkerRatingService workerRatingService,
    IAuthorizationService authorizationService) : ControllerBase
{
    public record VerbalRatingDto
    {
        public string? VerbalRating { get; set; }
    }

    //Get all ratings

    [HttpGet("/Employers/{employerId}/Ratings")]
    [AllowAnonymous]
    public async Task<IEnumerable<EmployerRatingDto>?> GetEmployerRatingsAsync(string employerId)
    {
        var serviceResult = await employerRatingService.GetAllAsync(employerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("/Workers/{workerId}/Ratings")]
    [AllowAnonymous]
    public async Task<IEnumerable<WorkerRatingDto>?> GetWorkerRatingsAsync(string workerId)
    {
        var serviceResult = await workerRatingService.GetAllAsync(workerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("/Employers/{employerId}/Jobs/{jobId}/Ratings")]
    [AllowAnonymous]
    public async Task<IEnumerable<WorkerJobDto>?> GetJobRatingsAsync(string jobId)
    {
        var serviceResult = await workerJobService.GetAllByJobIdAsync(jobId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }


    //Get single rating
    [HttpGet("/Employers/{employerId}/Ratings/{workerId}")]
    [AllowAnonymous]
    public async Task<EmployerRatingDto?> GetEmployerRatingAsync(string employerId, string workerId)
    {
        var serviceResult = await employerRatingService.GetAsync(employerId, workerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("/Workers/{workerId}/Ratings/{employerId}")]
    [AllowAnonymous]
    public async Task<WorkerRatingDto?> GetWorkerRatingAsync(string workerId, string employerId)
    {
        var serviceResult = await workerRatingService.GetAsync(workerId, employerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("/Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}")]
    [AllowAnonymous]
    public async Task<WorkerJobDto?> GetJobRatingAsync(string jobId, string workerId)
    {
        var serviceResult = await workerJobService.GetAsync(workerId, jobId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }


    //Average ratings
    [HttpGet("/Employers/{employerId}/Average-rating")]
    [AllowAnonymous]
    public async Task<float?> GetEmployerAverageRatingAsync(string employerId)
    {
        var serviceResult = await employerRatingService.GetAverageAsync(employerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("/Workers/{workerId}/Average-rating")]
    [AllowAnonymous]
    public async Task<float?> GetWorkerAverageRatingAsync(string workerId)
    {
        var serviceResult = await workerRatingService.GetAverageAsync(workerId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpGet("/Employers/{employerId}/Jobs/{jobId}/Average-rating")]
    [AllowAnonymous]
    public async Task<float?> GetJobAverageRatingAsync(string jobId)
    {
        var serviceResult = await workerJobService.GetAverageRatingByJobIdAsync(jobId);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
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

        var ratingDto = CreateRatingDto(employerId, workerId, numericalRating, verbalRating?.VerbalRating);
        
        var serviceResult = await employerRatingService.AddAsync(ratingDto);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpPost("/Workers/{workerId}/Ratings")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<WorkerRatingDto?> PostWorkerRatingAsync(string workerId,
        [Required] [FromQuery] string employerId,
        [Required] [FromQuery] [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
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
        var ratingDto = CreateRatingDto(employerId, workerId, numericalRating, verbalRating?.VerbalRating);
        
        var serviceResult = await workerRatingService.AddAsync(ratingDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }


    [HttpPatch("/Employers/{employerId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<EmployerRatingDto?> PatchEmployerRating(string employerId, string workerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
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

        var ratingDto = CreateRatingDto(employerId, workerId, numericalRating, verbalRating?.VerbalRating);
        
        var serviceResult = await employerRatingService.UpdateAsync(ratingDto);
        if (!serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpPatch("/Workers/{workerId}/Ratings/{employerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task<WorkerRatingDto?> PatchWorkerRating(string workerId, string employerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
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
        var ratingDto = CreateRatingDto(employerId, workerId, numericalRating, verbalRating?.VerbalRating);
        
        var serviceResult = await workerRatingService.UpdateAsync(ratingDto);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }

    [HttpPatch("/Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task<WorkerJobDto?> PatchJobRating(string employerId, string jobId, string workerId,
        [Required] [FromQuery] [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10")]
        int numericalRating,
        [FromBody] VerbalRatingDto? verbalRating)
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
        
        var serviceResult = 
            await workerJobService.UpdateRatingAsync(workerId, jobId, numericalRating, verbalRating?.VerbalRating);
        if (! serviceResult.IsSuccess) SetStatusCodeTo(GetStatusCode.BasedOnError(serviceResult.Error));
        return serviceResult.Data;
    }


    [HttpDelete("/Employers/{employerId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task DeleteEmployerRating(string employerId, string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return;
        }
        if (!await IsSameUserOrAdminAsync(workerId))
        {
            SetStatusCodeTo(401);
            return;
        }
        
        var serviceResult = await employerRatingService.DeleteAsync(employerId, workerId);
        SetStatusCodeTo(serviceResult.IsSuccess ? 
            204 : 
            GetStatusCode.BasedOnError(serviceResult.Error));
    }

    [HttpDelete("/Workers/{workerId}/Ratings/{employerId}")]
    [Authorize(Roles = "Employer,Admin")]
    public async Task DeleteWorkerRating(string workerId, string employerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return;
        }
        if (!await IsSameUserOrAdminAsync(employerId))
        {
            SetStatusCodeTo(401);
            return;
        }
        
        var serviceResult = await workerRatingService.DeleteAsync(workerId, employerId);
        SetStatusCodeTo(serviceResult.IsSuccess ? 
            204 : 
            GetStatusCode.BasedOnError(serviceResult.Error));
    }

    [HttpDelete("/Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}")]
    [Authorize(Roles = "Worker,Admin")]
    public async Task DeleteJobRating(string employerId, string jobId, string workerId)
    {
        if (!ModelState.IsValid)
        {
            SetStatusCodeTo(400);
            return;
        }
        if (!await IsSameUserOrAdminAsync(workerId))
        {
            SetStatusCodeTo(401);
            return;
        }
        
        var serviceResult = 
            await workerJobService.DeleteRatingAsync(workerId, jobId);
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

    private RatingDto CreateRatingDto(string employerId, string workerId, int numericalRating, string? verbalRating)
    {
        return new RatingDto
        {
            EmployerId = employerId,
            WorkerId = workerId,
            NumericalRating = numericalRating,
            VerbalRating = verbalRating,
        };
    }
}