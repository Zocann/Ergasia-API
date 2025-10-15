using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class WorkerRatingEfRepository(PrimaryDbContext context) : IWorkerRatingRepository
{
    public async IAsyncEnumerable<WorkerRating?> GetAllByIdAsync(string workerId)
    {
        await foreach (var workerRating in context.WorkerRatings
            .Where(wr => wr.WorkerId == workerId)
            .Include(wr => wr.Employer)
            .Include(wr => wr.Worker)
            .AsAsyncEnumerable())
        {
            yield return workerRating;
        }
    }

    public async Task<WorkerRating?> GetAsync(string workerId, string employerId)
    {
        return await context.WorkerRatings
            .Where(wr => wr.EmployerId == employerId && wr.WorkerId == workerId)
            .Include(wr => wr.Employer)
            .Include(wr => wr.Worker)
            .FirstOrDefaultAsync();
    }


    public async Task<WorkerRating?> AddAsync(string workerId, string employerId, int numericalRating, string? verbalRating)
    {
        if (await GetAsync(workerId, employerId) != null) return null;
        
        var employer = await context.Employers.FindAsync(employerId);
        var worker = await context.Workers.FindAsync(workerId);
        
        if (worker == null || employer == null) return null;
        
        var workerRepository = new WorkerRating()
        {
            NumericalRating = numericalRating,
            VerbalRating = verbalRating,
            EmployerId = employerId,
            WorkerId = workerId,
            Employer = employer,
            Worker = worker,
        };
        
        await context.WorkerRatings.AddAsync(workerRepository);
        await context.SaveChangesAsync();
        
        return workerRepository;
    }

    public async Task<WorkerRating?> UpdateAsync(string workerId, string employerId, int numericalRating, string? verbalRating)
    {
        var workerRating = await GetAsync(workerId, employerId);
        if (workerRating == null) return null;
        
        workerRating.NumericalRating = numericalRating;
        workerRating.VerbalRating = verbalRating;
        workerRating.Date = DateTime.Now;

        await context.SaveChangesAsync();
        return workerRating;
    }

    public async Task<bool> DeleteAsync(string workerId, string employerId)
    {
        var workerRating = await GetAsync(workerId, employerId);
        if (workerRating == null) return false;
        
        context.WorkerRatings.Remove(workerRating);
        await context.SaveChangesAsync();
        return true;
    }
}