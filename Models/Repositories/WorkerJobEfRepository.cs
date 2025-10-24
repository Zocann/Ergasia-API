using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class WorkerJobEfRepository(PrimaryDbContext context) : IWorkerJobRepository
{
    public async Task<List<WorkerJob>> GetByJobIdAsync(string jobId)
    {
        return await context.WorkerJobs
            .Where(wj => wj.JobId == jobId)
            .Include(wj => wj.Job)
            .Include(wj => wj.Worker)
            .ToListAsync();
    }

    public async Task<List<WorkerJob>> GetByWorkerIdAsync(string workerId)
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

    public async Task<List<WorkerJob>> GetByEmployerIdAsync(string employerId, string jobId)
    {
        return await context.WorkerJobs
            .Where(wj => wj.JobId == jobId && wj.Job.EmployerId == employerId)
            .Include(wj => wj.Job)
            .Include(wj => wj.Worker)
            .ToListAsync();
    }

    public async Task<WorkerJob?> AddAsync(string workerId, string jobId)
    {
        //Checking if the WorkerJob doesn't already exist
        var workerJob = await GetAsync(workerId, jobId);
        if (workerJob != null) return null;
        
        var job = await context.Jobs.FindAsync(jobId);
        if (job == null) return null;

        var worker = await context.Workers.FindAsync(workerId);
        if (worker == null) return null;
        
        workerJob = new WorkerJob
       {
           JobId = jobId,
           WorkerId = workerId,
           Job = job,
           Worker = worker
       };

       await context.WorkerJobs.AddAsync(workerJob);
       await context.SaveChangesAsync();
       
       return workerJob;
    }

    public async Task<WorkerJob?> UpdateRatingAsync(string workerId, string jobId, int numericalRating, string? verbalRating)
    {
        var workerJob = await GetAsync(workerId, jobId);

        if (workerJob == null) return null;
        
        workerJob.NumericalRating = numericalRating;
        workerJob.VerbalRating = verbalRating;
        workerJob.DateOfRating = DateTime.Now;
        
        await context.SaveChangesAsync();
        return workerJob;
    }

    public async Task<bool> DeleteAsync(string workerId, string jobId)
    {
        var workerJob = await GetAsync(workerId, jobId);

        if (workerJob == null) return false;
        
        context.WorkerJobs.Remove(workerJob);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRatingAsync(string workerId, string jobId)
    {
        var workerJob = await GetAsync(workerId, jobId);

        if (workerJob == null) return false;
        
        workerJob.NumericalRating = null;
        workerJob.VerbalRating = null;
        workerJob.DateOfRating = null;
        
        await context.SaveChangesAsync();
        return true;
    }
}