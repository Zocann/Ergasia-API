using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Rating;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class EmployerRatingService(IEmployerRatingRepository repository, IWorkerRepository workerRepository, 
    IEmployerRepository employerRepository, IJobRepository jobRepository, IWorkerJobRepository workerJobRepository,
    IMapper mapper) : IEmployerRatingService
{
    public async Task<ServiceResult<IEnumerable<EmployerRatingDto>>> GetAllAsync(string employerId)
    {
        try
        {
            var ratings = (await repository.GetAllByEmployerIdAsync(employerId)).ToList();
            if (ratings.Count == 0) return ServiceResultBuilder.BuildFailure<IEnumerable<EmployerRatingDto>>(ServiceResultError.EmptyCollection);

            var ratingsDto = MapEmployerRatingsToDtos(ratings);
            return ServiceResultBuilder.BuildSuccess(ratingsDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<EmployerRatingDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<EmployerRatingDto>> GetAsync(string employerId, string workerId)
    {
        try
        {
            var rating = await repository.GetAsync(employerId, workerId);
            if (rating == null) return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.NotFound);

            var ratingDto = MapEmployerRatingToDto(rating);
            return ServiceResultBuilder.BuildSuccess(ratingDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<float>> GetAverageAsync(string employerId)
    {
        try
        {
            var ratings = (await repository.GetAllByEmployerIdAsync(employerId)).ToList();
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

    public async Task<ServiceResult<EmployerRatingDto>> AddAsync(RatingDto ratingDto)
    {
        try
        {
            var rating = await repository.GetAsync(ratingDto.EmployerId , ratingDto.WorkerId);
            if (rating != null) return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.DuplicitId);

            if (! await IsEmployersPastWorker(ratingDto.EmployerId, ratingDto.WorkerId))
                return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.UnableToChange);
            
            var worker = await workerRepository.GetByIdAsync(ratingDto.WorkerId);
            var employer = await employerRepository.GetByIdAsync(ratingDto.EmployerId);

            if (worker == null || employer == null)
                return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.DependenciesNotFound);
            
            var newRating = CreateRating(ratingDto, employer, worker);
            
            await repository.AddAsync(newRating);
            return ServiceResultBuilder.BuildSuccess(MapEmployerRatingToDto(newRating));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<EmployerRatingDto>> UpdateAsync(RatingDto ratingDto)
    {
        try
        {
            var rating = await repository.GetAsync(ratingDto.EmployerId , ratingDto.WorkerId);
            if (rating == null) return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.NotFound);
            
            var worker = await workerRepository.GetByIdAsync(ratingDto.WorkerId);
            var employer = await employerRepository.GetByIdAsync(ratingDto.EmployerId);

            if (worker == null || employer == null)
                return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.DependenciesNotFound);
            
            var newRating = CreateRating(ratingDto, employer, worker);
            
            await repository.UpdateAsync(newRating);
            return ServiceResultBuilder.BuildSuccess(MapEmployerRatingToDto(newRating));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<EmployerRatingDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string employerId, string workerId)
    {
        try
        {
            var rating = await repository.GetAsync(employerId, workerId);
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
    
    private IEnumerable<EmployerRatingDto> MapEmployerRatingsToDtos(IEnumerable<EmployerRating> employerRatings)
    {
        List<EmployerRatingDto> result = [];
        result.AddRange(employerRatings.Select(mapper.Map<EmployerRatingDto>));
        return result;
    }

    private EmployerRatingDto MapEmployerRatingToDto(EmployerRating employerRating)
    {
        return mapper.Map<EmployerRatingDto>(employerRating);
    }

    private EmployerRating CreateRating(RatingDto ratingDto, Employer employer, Worker worker)
    {
        return new EmployerRating
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