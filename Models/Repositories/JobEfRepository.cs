using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class JobEfRepository(PrimaryDbContext context, IMapper mapper) : IJobRepository
{
    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await context.Jobs.ToListAsync();
    }
    
    public async Task<Job?> GetByIdAsync(string id)
    {
        var jobs = (await GetAllAsync()).ToList();
        
        return jobs.Count == 0 ? null : jobs.FirstOrDefault(job => job.Id == id);
    }

    public async Task<IEnumerable<Job>> GetByEmployerIdAsync(string employerId)
    {
        return await context.Jobs
            .Include(j => j.Employer)
            .Where(j => j.EmployerId == employerId)
            .ToListAsync();
    }

    public async Task AddAsync(Job job)
    {
        await context.Jobs.AddAsync(job);
        await context.SaveChangesAsync();
    }

    //MOve to services
    public async Task<Job?> CreateCopyAsync(string originalId)
    {
        var oldJob = await GetByIdAsync(originalId);
        if (oldJob == null) return null;
        
        var newJob = mapper.Map<Job>(oldJob);
        newJob.Id = Guid.NewGuid().ToString();
        newJob.DateOfBegin = DateTime.Now;
        
        await AddAsync(newJob);
        return newJob;
    }

    public async Task UpdateAsync(Job newJob)
    {
        var oldJob = await GetByIdAsync(newJob.Id);
        if (oldJob == null) return;
        
        mapper.Map(newJob, oldJob);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Job job)
    {
        context.Jobs.Remove(job);
        await context.SaveChangesAsync();
    }

    //Move to services
    public async Task<int?> AvailableWorkSpots(string id)
    {
        var job = await GetByIdAsync(id);
        if (job == null) return null;
        
        var workerJobs = context.WorkerJobs.Where(wj => wj.JobId == id).AsAsyncEnumerable();

        var count = 0;
        await foreach (var unused in workerJobs)
        {
            count++;
        }
        return job.WorkSpots - count;
    }
}