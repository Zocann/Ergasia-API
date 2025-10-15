namespace Ergasia_API.Models.Interfaces;

public interface IWorkerRatingRepository
{
    public IAsyncEnumerable<WorkerRating?> GetAllByIdAsync(string workerId);
    public Task<WorkerRating?> GetAsync(string workerId, string employerId);
    public Task<WorkerRating?> AddAsync(string workerId, string employerId, int numericalRating, string? verbalRating);
    public Task<WorkerRating?> UpdateAsync(string workerId, string employerId, int numericalRating, string? verbalRating);
    public Task<bool> DeleteAsync(string workerId, string employerId);
}