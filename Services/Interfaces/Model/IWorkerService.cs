using Ergasia_API.Data;
using Ergasia_API.DTOs.Worker;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IWorkerService
{
    public Task<ServiceResult<IEnumerable<WorkerDto>>> GetAllAsync();
    public Task<ServiceResult<WorkerDto>> GetByIdAsync(string id);
    public Task<ServiceResult<WorkerDto>> UpdateAsync(WorkerDto workerDto);
}