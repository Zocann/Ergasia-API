using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Rating;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class WorkerRatingService(IWorkerRatingRepository repository, IEmployerRepository employerRepository, 
    IWorkerRepository workerRepository, IJobRepository jobRepository, 
    IWorkerJobRepository workerJobRepository, IMapper mapper) : IWorkerRatingService
{
    public async Task<ServiceResult<IEnumerable<WorkerRatingDto>>> GetAllAsync(string workerId)
    {
        try
        {
            var ratings = (await repository.GetAllByWorkerIdAsync(workerId)).ToList();
            if (ratings.Count == 0) return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerRatingDto>>(ServiceResultError.EmptyCollection);

            var ratingsDto = MapWorkerRatingsToDtos(ratings);
            return ServiceResultBuilder.BuildSuccess(ratingsDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerRatingDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerRatingDto>> GetAsync(string workerId, string employerId)
    {
        try
        {
            var rating = await repository.GetAsync(workerId, employerId);
            if (rating == null) return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.NotFound);

            var ratingDto = MapWorkerRatingToDto(rating);
            return ServiceResultBuilder.BuildSuccess(ratingDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<float>> GetAverageAsync(string workerId)
    {
        try
        {
            var ratings = (await repository.GetAllByWorkerIdAsync(workerId)).ToList();
            if (ratings.Count == 0) return ServiceResultBuilder.BuildFailure<float>(ServiceResultError.EmptyCollection);

            var average = (float)ratings.Average(r => r.NumericalRating);
            return ServiceResultBuilder.BuildSuccess(average);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<float>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerRatingDto>> AddAsync(RatingDto ratingDto)
    {
        try
        {
            var rating = await repository.GetAsync(ratingDto.WorkerId, ratingDto.EmployerId);
            if (rating != null) return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.DuplicitId);

            if (! await IsEmployersPastWorker(ratingDto.EmployerId, ratingDto.WorkerId))
                return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.UnableToChange);
            
            var worker = await workerRepository.GetByIdAsync(ratingDto.WorkerId);
            var employer = await employerRepository.GetByIdAsync(ratingDto.EmployerId);

            if (worker == null || employer == null)
                return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.DependenciesNotFound);
            
            var newRating = CreateRating(ratingDto, employer, worker);
            
            await repository.AddAsync(newRating);
            return ServiceResultBuilder.BuildSuccess(MapWorkerRatingToDto(newRating));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerRatingDto>> UpdateAsync(RatingDto ratingDto)
    {
        try
        {
            var rating = await repository.GetAsync(ratingDto.WorkerId, ratingDto.EmployerId);
            if (rating == null) return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.NotFound);
            
            var worker = await workerRepository.GetByIdAsync(ratingDto.WorkerId);
            var employer = await employerRepository.GetByIdAsync(ratingDto.EmployerId);

            if (worker == null || employer == null)
                return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.DependenciesNotFound);
            
            var newRating = CreateRating(ratingDto, employer, worker);
            
            await repository.UpdateAsync(newRating);
            return ServiceResultBuilder.BuildSuccess(MapWorkerRatingToDto(newRating));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerRatingDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string workerId, string employerId)
    {
        try
        {
            var rating = await repository.GetAsync(workerId, employerId);
            if (rating == null) return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.NotFound);

            await repository.DeleteAsync(rating);
            return ServiceResultBuilder.BuildSuccess(true);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<bool>(ServiceResultError.DatabaseError);
        }
    }
    
    
    //Helpers
    private IEnumerable<WorkerRatingDto> MapWorkerRatingsToDtos(IEnumerable<WorkerRating> workerRatings)
    {
        List<WorkerRatingDto> result = [];
        result.AddRange(workerRatings.Select(mapper.Map<WorkerRatingDto>));
        return result;
    }

    private WorkerRatingDto MapWorkerRatingToDto(WorkerRating workerRating)
    {
        return mapper.Map<WorkerRatingDto>(workerRating);
    }

    private WorkerRating CreateRating(RatingDto ratingDto, Employer employer, Worker worker)
    {
        return new WorkerRating
        {
            NumericalRating = ratingDto.NumericalRating,
            VerbalRating = ratingDto.VerbalRating,
            EmployerId = ratingDto.EmployerId,
            WorkerId = ratingDto.WorkerId,
            Worker = worker,
            Employer = employer,
        };
    }
    
    private async Task<bool> IsEmployersPastWorker(string employerId, string workerId)
    {
        var jobs = (await jobRepository.GetByEmployerIdAsync(employerId)).ToList();
        if (jobs.Count <= 0) return false;
        
        foreach (var job in jobs)
        {
            if (await workerJobRepository.GetAsync(workerId, job.Id) != null) return true;
        }
        return false;
    }
}