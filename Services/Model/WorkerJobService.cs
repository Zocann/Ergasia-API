using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Job;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class WorkerJobService(
    IWorkerJobRepository repository, 
    IWorkerJobRequestRepository workerJobRequestRepository,
    IJobRepository jobRepository, 
    IWorkerRepository workerRepository, 
    IMapper mapper) : IWorkerJobService
{
    public async Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetAllByJobIdAsync(string jobId)
    {
        try
        {
            var workerJobs = (await repository.GetByJobIdAsync(jobId)).ToList();
            if (workerJobs.Count == 0) 
                return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerJobDto>>(ServiceResultError.EmptyCollection);

            var workerJobDtos = MapWorkerJobsToDtos(workerJobs);
            return ServiceResultBuilder.BuildSuccess(workerJobDtos);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerJobDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetAllByWorkerIdAsync(string workerId)
    {
        try
        {
            var workerJobs = (await repository.GetByWorkerIdAsync(workerId)).ToList();
            if (workerJobs.Count == 0) 
                return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerJobDto>>(ServiceResultError.EmptyCollection);

            var workerJobDtos = MapWorkerJobsToDtos(workerJobs);
            return ServiceResultBuilder.BuildSuccess(workerJobDtos);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerJobDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerJobDto>> GetAsync(string workerId, string jobId)
    {
        try
        {
            var workerJob = await GetWorkerJobAsync(workerId, jobId);
            if (workerJob == null) 
                return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.NotFound);

            var workerJobDto = MapWorkerJobToDto(workerJob);
            return ServiceResultBuilder.BuildSuccess(workerJobDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<float>> GetAverageRatingByJobIdAsync(string jobId)
    {
        try
        {
            var ratings = (await repository.GetByJobIdAsync(jobId)).ToList();
            if (ratings.Count == 0) return ServiceResultBuilder.BuildFailure<float>(ServiceResultError.EmptyCollection);
            
            var average = ratings.Average(wj => wj.NumericalRating);
            return average == null ? 
                ServiceResultBuilder.BuildFailure<float>(ServiceResultError.EmptyCollection) : 
                ServiceResultBuilder.BuildSuccess((float)average);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<float>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerJobDto>> AddAsync(string workerId, string jobId)
    {
        try
        {
            var job = await jobRepository.GetByIdAsync(jobId);
            if (job == null) return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DependenciesNotFound);
            
            var worker = await workerRepository.GetByIdAsync(workerId);
            if (worker == null) return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DependenciesNotFound);
            
            if (await GetWorkerJobAsync(workerId, jobId) != null) 
                return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DuplicitId);
            
            var workerJobRequest = await workerJobRequestRepository.GetAsync(workerId, jobId);
            if (workerJobRequest == null) 
                return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DependenciesNotFound);
            
            var workerJob = CreateWorkerJob(workerId, jobId, worker, job);
            
            await repository.AddAsync(workerJob);
            
            //First delete workerJobRequest
            await workerJobRequestRepository.DeleteAsync(workerJobRequest);
        
            return ServiceResultBuilder.BuildSuccess(MapWorkerJobToDto(workerJob));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerJobDto>> UpdateRatingAsync(string workerId, string jobId, int numericalRating, string? verbalRating)
    {
        try
        {
            var workerJob = await GetWorkerJobAsync(workerId, jobId);
            if (workerJob == null) return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.NotFound);
            
            var job = await jobRepository.GetByIdAsync(jobId);
            if (job == null) return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DependenciesNotFound);
            
            var worker = await workerRepository.GetByIdAsync(workerId);
            if (worker == null) return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DependenciesNotFound);
            
            workerJob.NumericalRating = numericalRating;
            workerJob.VerbalRating = verbalRating;
            
            await repository.UpdateRatingAsync(workerJob);
        
            return ServiceResultBuilder.BuildSuccess(MapWorkerJobToDto(workerJob));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerJobDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteRatingAsync(string workerId, string jobId)
    {
        try
        {
            var workerJob = await GetWorkerJobAsync(workerId, jobId);
            if (workerJob == null) 
                return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.NotFound);
            if (WorkerJobIsInProgresOrFinished(workerJob))
                return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.UnableToChange);

            await repository.DeleteRatingAsync(workerJob);
            return ServiceResultBuilder.BuildSuccess(true);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }
    
    private IEnumerable<WorkerJobDto> MapWorkerJobsToDtos(List<WorkerJob> workerJobs)
    {
        var workerJobDtos = new List<WorkerJobDto>(workerJobs.Count);
        workerJobDtos.AddRange(from workerJob in workerJobs select mapper.Map<WorkerJobDto>(workerJob));
        return workerJobDtos;
    }
    
    private WorkerJobDto MapWorkerJobToDto(WorkerJob workerJob)
    {
        return mapper.Map<WorkerJobDto>(workerJob);
    }
    
    private async Task<WorkerJob?> GetWorkerJobAsync(string workerId, string jobId)
    {
        return await repository.GetAsync(workerId, jobId);
    }

    private static bool WorkerJobIsInProgresOrFinished(WorkerJob workerJob)
    {
        return workerJob.Job.DateOfBegin <= DateTime.Now;
    }

    private static WorkerJob CreateWorkerJob(string workerId, string jobId, Worker worker, Job job)
    {
        return new WorkerJob
        {
            WorkerId = workerId,
            JobId = jobId,
            Job = job,
            Worker = worker
        };
    }
}