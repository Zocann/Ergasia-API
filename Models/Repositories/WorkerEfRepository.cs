using Ergasia_API.Data;
using Ergasia_API.Models.Interfaces;

namespace Ergasia_API.Models.Repositories;

public class WorkerEfRepository(PrimaryDbContext context) : IWorkerRepository
{
    public async Task<Worker?> GetByIdAsync(string id)
    {
        return await context.Workers.FindAsync(id);
    }

    public async IAsyncEnumerable<Worker?> GetAllAsync()
    {
        await foreach (var worker in context.Workers.AsAsyncEnumerable())
        {
            yield return worker;
        }
    }

    public async Task<Worker?> UpdateAsync(Worker worker)
    {
        if(await GetByIdAsync(worker.Id) == null) return null;
        
        context.Workers.Update(worker);
        await context.SaveChangesAsync();
        return worker;
    }
}