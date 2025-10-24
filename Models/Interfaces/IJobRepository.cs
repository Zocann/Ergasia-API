namespace Ergasia_API.Models.Interfaces;

public interface IJobRepository
{
    public Task<List<Job>> GetAllAsync();
    public Task<Job?> GetByIdAsync(string id);
    public Task<List<Job>> GetByEmployerIdAsync(string employerId);
    public Task<Job?> CreateCopyAsync(string originalId);
    public Task<Job> AddAsync(Job job);
    public Task<Job?> UpdateAsync(Job job);
    public Task<bool> DeleteAsync(string id);
    public Task<int?> AvailableWorkSpots(string id);
}