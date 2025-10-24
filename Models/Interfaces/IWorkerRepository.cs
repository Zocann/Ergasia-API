namespace Ergasia_API.Models.Interfaces;

public interface IWorkerRepository
{
    public Task<List<Worker>> GetAllAsync();
    public Task<Worker?> GetByIdAsync(string id);
    public Task<Worker?> UpdateAsync(Worker worker);
}