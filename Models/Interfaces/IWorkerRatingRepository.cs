namespace Ergasia_API.Models.Interfaces;

public interface IWorkerRatingRepository
{
    public Task<IEnumerable<WorkerRating>> GetAllByWorkerIdAsync(string workerId);
    public Task<WorkerRating?> GetAsync(string workerId, string employerId);
    public Task AddAsync(WorkerRating workerRating);
    public Task UpdateAsync(WorkerRating workerRating);
    public Task DeleteAsync(WorkerRating workerRating);
}