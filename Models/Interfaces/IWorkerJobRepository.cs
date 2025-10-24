namespace Ergasia_API.Models.Interfaces;

public interface IWorkerJobRepository
{
    public Task<List<WorkerJob>> GetByJobIdAsync(string jobId);
    public Task<List<WorkerJob>> GetByWorkerIdAsync(string workerId);
    public Task<WorkerJob?> GetAsync(string workerId, string jobId);
    public Task<List<WorkerJob>> GetByEmployerIdAsync(string employerId, string jobId);
    public Task<WorkerJob?> AddAsync(string workerId, string jobId);
    public Task<WorkerJob?> UpdateRatingAsync(string workerId, string jobId, int numericalRating, string? verbalRating);
    public Task<bool> DeleteAsync(string workerId, string jobId);
    public Task<bool> DeleteRatingAsync(string workerId, string jobId);
}