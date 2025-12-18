using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Job;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class JobService(IJobRepository jobRepository, IEmployerRepository employerRepository, IMapper mapper) : IJobService
{
    public async Task<ServiceResult<IEnumerable<JobDto>>> GetAllUpcomingAsync()
    {
        try
        {
            List<JobDto> result = [];
            var jobs = (await jobRepository.GetAllAsync()).ToList();

            if (jobs.Count == 0)
                return ServiceResultBuilder.BuildFailure<IEnumerable<JobDto>>(ServiceResultError.EmptyCollection);
            
            result.AddRange(from job in jobs where job.DateOfBegin > DateTime.UtcNow select mapper.Map<JobDto>(job));
            return ServiceResultBuilder.BuildSuccess<IEnumerable<JobDto>>(result);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<JobDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<IEnumerable<JobDto>>> GetAllFromEmployerAsync(string employerId)
    {
        try
        {
            List<JobDto> result = [];
            var jobs = (await jobRepository.GetByEmployerIdAsync(employerId)).ToList();

            if (jobs.Count == 0)
                return ServiceResultBuilder.BuildFailure<IEnumerable<JobDto>>(ServiceResultError.EmptyCollection);
            
            result.AddRange(from job in jobs select mapper.Map<JobDto>(job));
            return ServiceResultBuilder.BuildSuccess<IEnumerable<JobDto>>(result);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<JobDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<JobDto>> GetAsync(string id)
    {
        try
        {
            var job = await jobRepository.GetByIdAsync(id);

            return job == null ? 
                ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.NotFound) : 
                ServiceResultBuilder.BuildSuccess(mapper.Map<JobDto>(job));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<int>> GetJobWorkSpots(string id)
    {
        try
        {
            var workSpots = await jobRepository.AvailableWorkSpots(id);
        
            return workSpots == null ? 
                ServiceResultBuilder.BuildFailure<int>(ServiceResultError.NotFound) :
                ServiceResultBuilder.BuildSuccess((int)workSpots);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<int>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<JobDto>> AddAsync(JobDto jobDto)
    {
        try
        {
            if (jobDto.EmployerId == null)
                return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.InvalidArgument);
            var employer = await employerRepository.GetByIdAsync(jobDto.EmployerId);
            if (employer == null) return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.DependenciesNotFound);

            jobDto.Id = Guid.NewGuid().ToString();
            var job = mapper.Map<Job>(jobDto);
            job.Employer = employer;
        
            await jobRepository.AddAsync(job);

            return ServiceResultBuilder.BuildSuccess(jobDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<JobDto>> UpdateAsync(JobDto jobDto)
    {
        try
        {
            if (jobDto.Id == null)
                return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.InvalidArgument);
            
            var job = await jobRepository.GetByIdAsync(jobDto.Id);
            if (job == null) return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.NotFound);
            
            await jobRepository.UpdateAsync(mapper.Map<Job>(jobDto));
            return ServiceResultBuilder.BuildSuccess(jobDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<JobDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string id)
    {
        try
        {
            var job = await jobRepository.GetByIdAsync(id);
            if (job == null) return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.NotFound);
            
            await jobRepository.DeleteAsync(job);
            return ServiceResultBuilder.BuildSuccess(true);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }
}