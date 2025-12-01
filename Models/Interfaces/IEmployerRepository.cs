namespace Ergasia_API.Models.Interfaces;

public interface IEmployerRepository
{
    public Task<IEnumerable<Employer>> GetAllAsync();
    public Task<Employer?> GetByIdAsync(string id);
    public Task UpdateAsync(Employer employer);
}