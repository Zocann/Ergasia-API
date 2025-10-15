using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;

namespace Ergasia_API.Models.Repositories;

public class EmployerEfRepository(PrimaryDbContext context) : IEmployerRepository
{
    public async Task<Employer?> GetByIdAsync(string id)
    {
        return await context.Employers.FindAsync(id);
    }

    public async IAsyncEnumerable<Employer?> GetAllAsync()
    {
        await foreach (var employer in context.Employers.AsAsyncEnumerable())
        {
            yield return employer;
        }
    }

    public async IAsyncEnumerable<Employer?> GetAllActiveAsync()
    {
        var employers = GetAllAsync();
        
        await foreach (var employer in employers)
        {
            yield return employer;
        }
    }
    
    
    public async Task<Employer?> UpdateAsync(Employer employer)
    {
        if (await GetByIdAsync(employer.Id) == null) return null;
        
        context.Employers.Update(employer);
        await context.SaveChangesAsync();
        return employer;
    }
}