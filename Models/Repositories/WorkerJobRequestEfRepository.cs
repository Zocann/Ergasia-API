using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class WorkerJobRequestEfRepository(PrimaryDbContext context) : IWorkerJobRequestRepository
{
    public async Task<WorkerJobRequest?> GetAsync(string workerId, string jobId)
    {
        return await context.WorkerJobRequests
            .Where(wj => wj.WorkerId == workerId && wj.JobId == jobId)
            .Include(wjr => wjr.Worker)
            .Include(wjr => wjr.Job)
            .SingleOrDefaultAsync(wjr => wjr.WorkerId == workerId && wjr.JobId == jobId);
    }

    public async Task<List<WorkerJobRequest>> GetByEmployerIdAsync(string employerId, string jobId)
    {
        return await context.WorkerJobRequests
            .Include(wjr => wjr.Worker)
            .Include(wjr => wjr.Job)
            .Where(wjr => wjr.JobId == jobId && wjr.Job.EmployerId == employerId)
            .ToListAsync();
    }
    
    public async Task<List<WorkerJobRequest>> GetByWorkerId(string workerId)
    {
        return await context.WorkerJobRequests
                           .Include(wjr => wjr.Worker)
                           .Include(wjr => wjr.Job)
                           .Where(wjr => wjr.WorkerId == workerId)
                           .ToListAsync();
    }

    public async Task<WorkerJobRequest?> AddAsync(string workerId, string jobId, string? message)
    {
        var wjr = await GetAsync(workerId, jobId);
        
        if (wjr != null) return null;
        
        var worker = await context.Workers.FindAsync(workerId);
        if (worker == null) return null;
        
        var job = await context.Jobs.FindAsync(jobId);
        if (job == null) return null;

        var workerJobRequest = new WorkerJobRequest
        {
            WorkerId = workerId,
            JobId = jobId,
            Message = message,
            Worker = worker,
            Job = job,
            ExpirationDate = DateTime.Now.AddDays(7)
        };
        
        await context.WorkerJobRequests.AddAsync(workerJobRequest);
        await context.SaveChangesAsync();
        
        return workerJobRequest;
    }

    public async Task<bool> DeleteAsync(string workerId, string jobId)
    {
        var wjr = await GetAsync(workerId, jobId);
        
        if (wjr == null) return false;
        
        context.WorkerJobRequests.Remove(wjr);
        await context.SaveChangesAsync();
        
        return true;
    }
}