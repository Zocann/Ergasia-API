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

    public async Task<IEnumerable<WorkerJobRequest>> GetByEmployerIdAsync(string employerId, string jobId)
    {
        return await context.WorkerJobRequests
            .Include(wjr => wjr.Worker)
            .Include(wjr => wjr.Job)
            .Where(wjr => wjr.JobId == jobId && wjr.Job.EmployerId == employerId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<WorkerJobRequest>> GetByWorkerId(string workerId)
    {
        return await context.WorkerJobRequests
                           .Include(wjr => wjr.Worker)
                           .Include(wjr => wjr.Job)
                           .Where(wjr => wjr.WorkerId == workerId)
                           .ToListAsync();
    }

    public async Task AddAsync(WorkerJobRequest workerJobRequest)
    {
        await context.WorkerJobRequests.AddAsync(workerJobRequest);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WorkerJobRequest workerJobRequest)
    {
        context.WorkerJobRequests.Remove(workerJobRequest);
        await context.SaveChangesAsync();
    }
}