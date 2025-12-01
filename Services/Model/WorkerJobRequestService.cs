using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Job;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class WorkerJobRequestService(IWorkerJobRequestRepository workerJobRequestRepository, 
    IJobRepository jobRepository, IWorkerRepository workerRepository, IMapper mapper) : IWorkerJobRequestService
{
    public async Task<ServiceResult<IEnumerable<JobRequestDto>>> GetAllFromEmployerAsync(string employerId, string jobId)
    {
        try
        {
            var jobRequests = (await workerJobRequestRepository.GetByEmployerIdAsync(employerId, jobId)).ToList();
            if (jobRequests.Count == 0) 
                return ServiceResultBuilder.BuildFailure<IEnumerable<JobRequestDto>>(ServiceResultError.EmptyCollection);
            
            var jobRequestDtos = MapJobRequestsToDtos(jobRequests);
            return ServiceResultBuilder.BuildSuccess(jobRequestDtos);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<JobRequestDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<IEnumerable<JobRequestDto>>> GetAllFromWorkerAsync(string workerId)
    {
        try
        {
            var jobRequests = (await workerJobRequestRepository.GetByWorkerId(workerId)).ToList();
            if (jobRequests.Count == 0) 
                return ServiceResultBuilder.BuildFailure<IEnumerable<JobRequestDto>>(ServiceResultError.EmptyCollection);

            var jobRequestDto = MapJobRequestsToDtos(jobRequests);
            return ServiceResultBuilder.BuildSuccess(jobRequestDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<JobRequestDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<JobRequestDto>> GetAsync(string workerId, string jobId)
    {
        try
        {
            var jobRequest = await GetJobRequestAsync(workerId, jobId);
            
            return jobRequest == null ? 
                ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.NotFound) :
                ServiceResultBuilder.BuildSuccess(MapJobRequestToDto(jobRequest));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<JobRequestDto>> AddAsync(string workerId, string jobId, string? message)
    {
        try
        {
            var job = await jobRepository.GetByIdAsync(jobId);
            if (job == null) return ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.DependenciesNotFound);
            
            var worker = await workerRepository.GetByIdAsync(workerId);
            if (worker == null) return ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.DependenciesNotFound);
            
            
            if (await GetJobRequestAsync(workerId, jobId) != null) 
                return ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.DuplicitId);
            
            if (JobIsFinished(job))
                return ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.UnableToChange);

            var jobRequest = new WorkerJobRequest
            {
                WorkerId = workerId,
                JobId = jobId,
                Message = message,
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                Job = job,
                Worker = worker
            };
            
            await workerJobRequestRepository.AddAsync(jobRequest);
        
            return ServiceResultBuilder.BuildSuccess(MapJobRequestToDto(jobRequest));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<JobRequestDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string workerId, string jobId)
    {
        try
        {
            var jobRequest = await GetJobRequestAsync(workerId, jobId);
            if (jobRequest == null)
                return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.NotFound);

            await workerJobRequestRepository.DeleteAsync(jobRequest);
            return ServiceResultBuilder.BuildSuccess(true);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }



    private async Task<WorkerJobRequest?> GetJobRequestAsync(string workerId, string jobId)
    {
        return await workerJobRequestRepository.GetAsync(workerId, jobId);
    }
    private IEnumerable<JobRequestDto> MapJobRequestsToDtos(List<WorkerJobRequest> jobRequests)
    {
        var jobRequestDto = new List<JobRequestDto>(jobRequests.Count);
        jobRequestDto.AddRange(from jobRequest in jobRequests select mapper.Map<JobRequestDto>(jobRequest));
        return jobRequestDto;
    }

    private JobRequestDto MapJobRequestToDto(WorkerJobRequest jobRequest)
    {
        return mapper.Map<JobRequestDto>(jobRequest);
    }

    private static bool JobIsFinished(Job job)
    {
        return job.DateOfBegin.AddDays(job.Duration) < DateTime.UtcNow;
    }
}