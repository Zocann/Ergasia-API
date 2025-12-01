using Ergasia_API.Data;
using Ergasia_API.DTOs.Employer;

namespace Ergasia_API.Services.Interfaces.Model;

public interface IEmployerService
{
    public Task<ServiceResult<IEnumerable<EmployerDto>>> GetAllAsync();
    public Task<ServiceResult<EmployerDto>> GetByIdAsync(string id);
    public Task<ServiceResult<EmployerDto>> UpdateAsync(EmployerDto employerDto);
}