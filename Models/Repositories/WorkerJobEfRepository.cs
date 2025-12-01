using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class WorkerJobEfRepository(PrimaryDbContext context, IMapper mapper) : IWorkerJobRepository
{
    public async Task<IEnumerable<WorkerJob>> GetByJobIdAsync(string jobId)
    {
        return await context.WorkerJobs
            .Where(wj => wj.JobId == jobId)
            .Include(wj => wj.Job)
            .Include(wj => wj.Worker)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkerJob>> GetByWorkerIdAsync(string workerId)
    {
        return await context.WorkerJobs
            .Where(wj => wj.WorkerId == workerId)
            .Include(wj => wj.Job)
            .Include(wj => wj.Worker)
            .ToListAsync();
    }

    public async Task<WorkerJob?> GetAsync(string workerId, string jobId)
    {
        return await context.WorkerJobs
            .Where(wj => wj.WorkerId == workerId && wj.JobId == jobId)
            .Include(wj => wj.Job)
            .Include(wj => wj.Worker)
            .FirstOrDefaultAsync();

    }

    public async Task<IEnumerable<WorkerJob>> GetByEmployerIdAsync(string employerId, string jobId)
    {
        return await context.WorkerJobs
            .Where(wj => wj.JobId == jobId && wj.Job.EmployerId == employerId)
            .Include(wj => wj.Job)
            .Include(wj => wj.Worker)
            .ToListAsync();
    }

    public async Task AddAsync(WorkerJob workerJob)
    {
       await context.WorkerJobs.AddAsync(workerJob);
       await context.SaveChangesAsync();
    }

    public async Task UpdateRatingAsync(WorkerJob newWorkerJob)
    {
        var workerJob = await GetAsync(newWorkerJob.WorkerId, newWorkerJob.JobId);
        if (workerJob == null) return;
        
        workerJob.NumericalRating = newWorkerJob.NumericalRating;
        workerJob.VerbalRating = newWorkerJob.VerbalRating;
        workerJob.DateOfRating = DateTime.Now;
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WorkerJob workerJob)
    {
        context.WorkerJobs.Remove(workerJob);
        await context.SaveChangesAsync();
    }

    public async Task DeleteRatingAsync(WorkerJob workerJobToDelete)
    {
        var workerJob = await GetAsync(workerJobToDelete.WorkerId, workerJobToDelete.JobId);
        if (workerJob == null) return;
        
        workerJob.NumericalRating = null;
        workerJob.VerbalRating = null;
        workerJob.DateOfRating = null;
        
        await context.SaveChangesAsync();
    }
}