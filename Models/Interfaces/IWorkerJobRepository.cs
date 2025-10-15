namespace Ergasia_API.Models.Interfaces;

public interface IWorkerJobRepository
{
    public IAsyncEnumerable<WorkerJob?> GetByJobIdAsync(string jobId);
    public IAsyncEnumerable<WorkerJob?> GetByWorkerIdAsync(string workerId);
    public Task<WorkerJob?> GetAsync(string workerId, string jobId);
    public IAsyncEnumerable<WorkerJob?> GetByEmployerIdAsync(string employerId, string jobId);
    public Task<WorkerJob?> AddAsync(string workerId, string jobId);
    public Task<WorkerJob?> UpdateRatingAsync(string workerId, string jobId, int numericalRating, string? verbalRating);
    public Task<bool> DeleteAsync(string workerId, string jobId);
    public Task<bool> DeleteRatingAsync(string workerId, string jobId);
}