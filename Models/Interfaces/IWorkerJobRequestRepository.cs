namespace Ergasia_API.Models.Interfaces;

public interface IWorkerJobRequestRepository
{
    public Task<WorkerJobRequest?> GetAsync(string workerId, string jobId);
    public Task<IEnumerable<WorkerJobRequest>> GetByEmployerIdAsync(string employerId, string jobId);
    public Task<IEnumerable<WorkerJobRequest>> GetByWorkerId(string workerId);
    public Task AddAsync(WorkerJobRequest workerJobRequest);
    public Task DeleteAsync(WorkerJobRequest workerJobRequest);
}