using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class EmployerEfRepository(PrimaryDbContext context) : IEmployerRepository
{
    public async Task<Employer?> GetByIdAsync(string id)
    {
        return await context.Employers.FindAsync(id);
    }

    public async Task<List<Employer>> GetAllAsync()
    {
        return await context.Employers.ToListAsync();
    }
    
    
    public async Task<Employer?> UpdateAsync(Employer employer)
    {
        if (await GetByIdAsync(employer.Id) == null) return null;
        
        context.Employers.Update(employer);
        await context.SaveChangesAsync();
        return employer;
    }
}