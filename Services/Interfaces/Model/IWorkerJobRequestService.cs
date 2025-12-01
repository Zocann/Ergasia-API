using Ergasia_API.Data;
using Ergasia_API.DTOs.Job;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IWorkerJobRequestService
{
    public Task<ServiceResult<IEnumerable<JobRequestDto>>> GetAllFromEmployerAsync(string employerId, string jobId);
    public Task<ServiceResult<IEnumerable<JobRequestDto>>> GetAllFromWorkerAsync(string workerId);
    public Task<ServiceResult<JobRequestDto>> GetAsync(string workerId, string jobId);
    public Task<ServiceResult<JobRequestDto>> AddAsync(string workerId, string jobId, string? message);
    public Task<ServiceResult<bool>> DeleteAsync(string workerId, string jobId);
}