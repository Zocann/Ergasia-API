using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class EmployerRatingEfRepository(PrimaryDbContext context) : IEmployerRatingRepository
{
    public async Task<IEnumerable<EmployerRating>> GetAllByEmployerIdAsync(string employerId)
    {
        return await context.EmployerRatings
            .Where(ej => ej.EmployerId == employerId)
            .Include(ej => ej.Employer)
            .Include(ej => ej.Worker)
            .ToListAsync();
    }

    public async Task<EmployerRating?> GetAsync(string employerId, string workerId)
    {
        return await context.EmployerRatings
            .Where(er => er.EmployerId == employerId && er.WorkerId == workerId)
            .Include(er => er.Employer)
            .Include(er => er.Worker)
            .SingleOrDefaultAsync();
    }

    public async Task AddAsync(EmployerRating employerRating)
    {
        await context.EmployerRatings.AddAsync(employerRating);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmployerRating employerRating)
    {
        var oldEmployerRating = await GetAsync(employerRating.EmployerId, employerRating.WorkerId);

        if (oldEmployerRating == null) return;
        
        oldEmployerRating.NumericalRating = employerRating.NumericalRating;
        oldEmployerRating.VerbalRating = employerRating.VerbalRating;
        oldEmployerRating.Date = DateTime.Now;
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(EmployerRating employerRating)
    {
        context.EmployerRatings.Remove(employerRating);
        await context.SaveChangesAsync();
    }
}