namespace Ergasia_API.Models.Interfaces;

public interface IWorkerJobRepository
{
    public Task<IEnumerable<WorkerJob>> GetByJobIdAsync(string jobId);
    public Task<IEnumerable<WorkerJob>> GetByWorkerIdAsync(string workerId);
    public Task<WorkerJob?> GetAsync(string workerId, string jobId);
    public Task<IEnumerable<WorkerJob>> GetByEmployerIdAsync(string employerId, string jobId);
    public Task AddAsync(WorkerJob workerJob);
    public Task UpdateRatingAsync(WorkerJob workerJob);
    public Task DeleteAsync(WorkerJob workerJob);
    public Task DeleteRatingAsync(WorkerJob workerJob);
}