using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Employer;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class EmployerService(IEmployerRepository repository, IMapper mapper) : IEmployerService
{
    public async Task<ServiceResult<IEnumerable<EmployerDto>>> GetAllAsync()
    {
        try
        {
            var employers = (await repository.GetAllAsync()).ToList();

            if (employers.Count == 0)
                return ServiceResultBuilder.BuildFailure<IEnumerable<EmployerDto>>(ServiceResultError.EmptyCollection);

            var employerDto = MapEmployersToDtos(employers);
            
            return ServiceResultBuilder.BuildSuccess(employerDto);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<EmployerDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<EmployerDto>> GetByIdAsync(string id)
    {
        try
        {
            var employer = await repository.GetByIdAsync(id);

            return employer == null ? 
                ServiceResultBuilder.BuildFailure<EmployerDto>(ServiceResultError.NotFound) : 
                ServiceResultBuilder.BuildSuccess(mapper.Map<EmployerDto>(employer));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<EmployerDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<EmployerDto>> UpdateAsync(EmployerDto employerDto)
    {
        try
        {
            var oldEmployer = await repository.GetByIdAsync(employerDto.Id);
            if (oldEmployer == null) return ServiceResultBuilder.BuildFailure<EmployerDto>(ServiceResultError.NotFound);
        
            var employer = MapEmployerDtoToEmployer(employerDto, oldEmployer);
            await repository.UpdateAsync(employer);
            return ServiceResultBuilder.BuildSuccess(mapper.Map<EmployerDto>(employer));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<EmployerDto>(ServiceResultError.DatabaseError);
        }
    }
    
    private IEnumerable<EmployerDto> MapEmployersToDtos(IEnumerable<Employer> employers)
    {
        List<EmployerDto> result = [];
        result.AddRange(employers.Select(mapper.Map<EmployerDto>));
        return result;
    }

    private Employer MapEmployerDtoToEmployer(EmployerDto employerDto, Employer oldEmployer)
    {
        var employer = mapper.Map<Employer>(employerDto);
        
        employer.Id = oldEmployer.Id;
        employer.RefreshToken = oldEmployer.RefreshToken;
        employer.RefreshTokenExpiration = oldEmployer.RefreshTokenExpiration;
        employer.PictureUrl = oldEmployer.PictureUrl;
        employer.IsActive = oldEmployer.IsActive;
        employer.DateOfRegistration = oldEmployer.DateOfRegistration;
        
        return employer;
    }
}