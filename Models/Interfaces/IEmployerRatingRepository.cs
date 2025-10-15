namespace Ergasia_API.Models.Interfaces;

public interface IEmployerRatingRepository
{
    public IAsyncEnumerable<EmployerRating?> GetAllByIdAsync(string employerId);
    public Task<EmployerRating?> GetAsync(string employerId, string workerId);
    public Task<EmployerRating?> AddAsync(string employerId, string workerId, int numericalRating, string? verbalRating);
    public Task<EmployerRating?> UpdateAsync(string employerId, string workerId, int numericalRating, string? verbalRating);
    public Task<bool> DeleteAsync(string employerId, string workerId);
}