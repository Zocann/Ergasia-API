using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class WorkerRatingEfRepository(PrimaryDbContext context) : IWorkerRatingRepository
{
    public async Task<IEnumerable<WorkerRating>> GetAllByWorkerIdAsync(string workerId)
    {
        return await context.WorkerRatings
            .Where(wr => wr.WorkerId == workerId)
            .Include(wr => wr.Employer)
            .Include(wr => wr.Worker)
            .ToListAsync();

    }

    public async Task<WorkerRating?> GetAsync(string workerId, string employerId)
    {
        return await context.WorkerRatings
            .Where(wr => wr.EmployerId == employerId && wr.WorkerId == workerId)
            .Include(wr => wr.Employer)
            .Include(wr => wr.Worker)
            .FirstOrDefaultAsync();
    }


    public async Task AddAsync(WorkerRating workerRating)
    {
        await context.WorkerRatings.AddAsync(workerRating);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WorkerRating newWorkerRating)
    {
        var workerRating = await GetAsync(newWorkerRating.WorkerId, newWorkerRating.EmployerId);
        if (workerRating == null) return;
        
        workerRating.NumericalRating = newWorkerRating.NumericalRating;
        workerRating.VerbalRating = newWorkerRating.VerbalRating;
        workerRating.Date = DateTime.Now;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WorkerRating workerRating)
    {
        context.WorkerRatings.Remove(workerRating);
        await context.SaveChangesAsync();
    }
}