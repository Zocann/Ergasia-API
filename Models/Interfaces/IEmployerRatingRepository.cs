namespace Ergasia_API.Models.Interfaces;

public interface IEmployerRatingRepository
{
    public Task<IEnumerable<EmployerRating>> GetAllByEmployerIdAsync(string employerId);
    public Task<EmployerRating?> GetAsync(string employerId, string workerId);
    public Task AddAsync(EmployerRating employerRating);
    public Task UpdateAsync(EmployerRating employerRating);
    public Task DeleteAsync(EmployerRating employerRating);
}