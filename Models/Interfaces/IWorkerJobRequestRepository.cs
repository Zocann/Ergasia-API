namespace Ergasia_API.Models.Interfaces;

public interface IWorkerJobRequestRepository
{
    public Task<WorkerJobRequest?> GetAsync(string workerId, string jobId);
    public Task<List<WorkerJobRequest>> GetByEmployerIdAsync(string employerId, string jobId);
    public Task<List<WorkerJobRequest>> GetByWorkerId(string workerId);
    public Task<WorkerJobRequest?> AddAsync(string workerId, string jobId, string? message);
    public Task<bool> DeleteAsync(string workerId, string jobId);
}