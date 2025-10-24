using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ergasia_API.Models.Repositories;

public class WorkerEfRepository(PrimaryDbContext context) : IWorkerRepository
{
    public async Task<Worker?> GetByIdAsync(string id)
    {
        return await context.Workers.FindAsync(id);
    }

    public async Task<List<Worker>> GetAllAsync()
    {
        return await context.Workers.ToListAsync();
    }

    public async Task<Worker?> UpdateAsync(Worker worker)
    {
        if(await GetByIdAsync(worker.Id) == null) return null;
        
        context.Workers.Update(worker);
        await context.SaveChangesAsync();
        return worker;
    }
}