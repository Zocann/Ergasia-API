using Ergasia_API.Data;
using Ergasia_API.DTOs.Job;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IWorkerJobService
{
    public Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetAllByJobIdAsync(string jobId);
    public Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetAllByWorkerIdAsync(string workerId);
    public Task<ServiceResult<WorkerJobDto>> GetAsync(string workerId, string jobId);
    public Task<ServiceResult<float>> GetAverageRatingByJobIdAsync(string jobId);
    public Task<ServiceResult<WorkerJobDto>> AddAsync(string workerId, string jobId);
    public Task<ServiceResult<WorkerJobDto>> UpdateRatingAsync(string workerId, string jobId, int numericalRating, string? verbalRating);
    public Task<ServiceResult<bool>> DeleteRatingAsync(string workerId, string jobId);
}