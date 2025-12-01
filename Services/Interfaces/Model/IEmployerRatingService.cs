using Ergasia_API.Data;
using Ergasia_API.DTOs.Rating;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IEmployerRatingService
{
    public Task<ServiceResult<IEnumerable<EmployerRatingDto>>> GetAllAsync(string employerId);
    public Task<ServiceResult<EmployerRatingDto>> GetAsync(string employerId, string workerId);
    public Task<ServiceResult<float>> GetAverageAsync(string employerId);
    public Task<ServiceResult<EmployerRatingDto>> AddAsync(RatingDto ratingDto);
    public Task<ServiceResult<EmployerRatingDto>> UpdateAsync(RatingDto ratingDto);
    public Task<ServiceResult<bool>> DeleteAsync(string employerId, string workerId);
}