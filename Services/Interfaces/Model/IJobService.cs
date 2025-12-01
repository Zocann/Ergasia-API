using Ergasia_API.Data;
using Ergasia_API.DTOs.Job;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IJobService
{
    public Task<ServiceResult<IEnumerable<JobDto>>> GetAllUpcomingAsync();
    public Task<ServiceResult<IEnumerable<JobDto>>> GetAllFromEmployerAsync(string employerId);
    public Task<ServiceResult<JobDto>> GetAsync(string id);
    public Task<ServiceResult<int>> GetJobWorkSpots(string id);
    public Task<ServiceResult<JobDto>> AddAsync(JobDto jobDto);
    public Task<ServiceResult<JobDto>> UpdateAsync(JobDto jobDto);
    public Task<ServiceResult<bool>> DeleteAsync(string id);
}