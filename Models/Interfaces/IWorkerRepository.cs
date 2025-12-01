namespace Ergasia_API.Models.Interfaces;

public interface IWorkerRepository
{
    public Task<IEnumerable<Worker>> GetAllAsync();
    public Task<Worker?> GetByIdAsync(string id);
    public Task UpdateAsync(Worker worker);
}