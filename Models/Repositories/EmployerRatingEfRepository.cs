using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class EmployerRatingEfRepository(PrimaryDbContext context) : IEmployerRatingRepository
{
    public async Task<List<EmployerRating>> GetAllByIdAsync(string employerId)
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

    public async Task<EmployerRating?> AddAsync(string employerId, string workerId, int numericalRating, string? verbalRating)
    {
        if (await GetAsync(workerId, employerId) != null) return null;
        
        var worker = await context.Workers.FindAsync(workerId);
        var employer = await context.Employers.FindAsync(employerId);
        
        if (worker == null || employer == null) return null;
        
        var employerRating = new EmployerRating()
        {
            NumericalRating = numericalRating,
            VerbalRating = verbalRating,
            EmployerId = employerId,
            WorkerId = workerId,
            Worker = worker,
            Employer = employer,
        };
        
        await context.EmployerRatings.AddAsync(employerRating);
        await context.SaveChangesAsync();
        
        return employerRating;
    }

    public async Task<EmployerRating?> UpdateAsync(string employerId, string workerId, int numericalRating, string? verbalRating)
    {
        var employerRating = await GetAsync(employerId, workerId);
        if (employerRating == null) return null;
        
        employerRating.NumericalRating = numericalRating;
        employerRating.VerbalRating = verbalRating;
        employerRating.Date = DateTime.Now;
        
        await context.SaveChangesAsync();
        return employerRating;
    }

    public async Task<bool> DeleteAsync(string employerId, string workerId)
    {
        var employerRating = await GetAsync(employerId, workerId);
        if (employerRating == null) return false;
        
        context.EmployerRatings.Remove(employerRating);
        await context.SaveChangesAsync();
        return true;
    }
}