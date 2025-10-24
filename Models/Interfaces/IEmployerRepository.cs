namespace Ergasia_API.Models.Interfaces;

public interface IEmployerRepository
{
    public Task<List<Employer>> GetAllAsync();
    public Task<Employer?> GetByIdAsync(string id);
    public Task<Employer?> UpdateAsync(Employer employer);
}