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

    public async Task<IEnumerable<Worker>> GetAllAsync()
    {
        return await context.Workers.ToListAsync();
    }

    public async Task UpdateAsync(Worker worker)
    {
        context.Workers.Update(worker);
        await context.SaveChangesAsync();
    }
}