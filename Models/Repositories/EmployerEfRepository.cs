using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class EmployerEfRepository(PrimaryDbContext context, IMapper mapper) : IEmployerRepository
{
    public async Task<Employer?> GetByIdAsync(string id)
    {
        return await context.Employers.FindAsync(id);
    }

    public async Task<IEnumerable<Employer>> GetAllAsync()
    {
        return await context.Employers.ToListAsync();
    }
    
    
    public async Task UpdateAsync(Employer newEmployer)
    {
        var employer = await GetByIdAsync(newEmployer.Id);
        mapper.Map(newEmployer, employer);
        await context.SaveChangesAsync();
    }
}