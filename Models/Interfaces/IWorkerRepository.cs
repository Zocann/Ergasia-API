namespace Ergasia_API.Models.Interfaces;

public interface IWorkerRepository
{
    public IAsyncEnumerable<Worker?> GetAllAsync();
    public Task<Worker?> GetByIdAsync(string id);
    public Task<Worker?> UpdateAsync(Worker worker);
}