namespace Ergasia_API.Models.Interfaces;

public interface IEmployerRepository
{
    public IAsyncEnumerable<Employer?> GetAllAsync();
    public Task<Employer?> GetByIdAsync(string id);
    public Task<Employer?> UpdateAsync(Employer employer);
}