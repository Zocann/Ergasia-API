namespace Ergasia_API.Models.Interfaces;

public interface IJobRepository
{
    public Task<IEnumerable<Job>> GetAllAsync();
    public Task<Job?> GetByIdAsync(string id);
    public Task<IEnumerable<Job>> GetByEmployerIdAsync(string employerId);
    public Task<Job?> CreateCopyAsync(string originalId);
    public Task AddAsync(Job job);
    public Task UpdateAsync(Job Job);
    public Task DeleteAsync(Job job);
    public Task<int?> AvailableWorkSpots(string id);
}