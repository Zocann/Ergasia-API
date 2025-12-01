using Ergasia_API.Data;
using Ergasia_API.DTOs.Rating;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IWorkerRatingService
{
    public Task<ServiceResult<IEnumerable<WorkerRatingDto>>> GetAllAsync(string workerId);
    public Task<ServiceResult<WorkerRatingDto>> GetAsync(string workerId, string employerId);
    public Task<ServiceResult<float>> GetAverageAsync(string workerId);
    public Task<ServiceResult<WorkerRatingDto>> AddAsync(RatingDto ratingDto);
    public Task<ServiceResult<WorkerRatingDto>> UpdateAsync(RatingDto ratingDto);
    public Task<ServiceResult<bool>> DeleteAsync(string workerId, string employerId);
}