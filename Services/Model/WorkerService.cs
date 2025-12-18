using AutoMapper;
using Ergasia_API.Data;
using Ergasia_API.Data.Enums;
using Ergasia_API.DTOs.Worker;
using Ergasia_API.Helpers;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Services.Interfaces.Model;

namespace Ergasia_API.Services.Model;

public class WorkerService(IWorkerRepository repository, IMapper mapper) : IWorkerService
{
    public async Task<ServiceResult<IEnumerable<WorkerDto>>> GetAllAsync()
    {
        try
        {
            var workers = (await repository.GetAllAsync()).ToList();

            if (workers.Count == 0)
                return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerDto>>(ServiceResultError.EmptyCollection);

            var workerDtos = MapWorkersToDtos(workers);
            
            return ServiceResultBuilder.BuildSuccess(workerDtos);
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<IEnumerable<WorkerDto>>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerDto>> GetByIdAsync(string id)
    {
        try
        {
            var worker = await repository.GetByIdAsync(id);

            return worker == null ? 
                ServiceResultBuilder.BuildFailure<WorkerDto>(ServiceResultError.NotFound) : 
                ServiceResultBuilder.BuildSuccess(mapper.Map<WorkerDto>(worker));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerDto>(ServiceResultError.DatabaseError);
        }
    }

    public async Task<ServiceResult<WorkerDto>> UpdateAsync(WorkerDto workerDto)
    {
        try
        {
            var oldWorker = await repository.GetByIdAsync(workerDto.Id);
            if (oldWorker == null) return ServiceResultBuilder.BuildFailure<WorkerDto>(ServiceResultError.NotFound);
        
            var worker = MapWorkerDtoToWorker(workerDto, oldWorker);
            await repository.UpdateAsync(worker);
            return ServiceResultBuilder.BuildSuccess(mapper.Map<WorkerDto>(worker));
        }
        catch (Exception e)
        {
            ExceptionService.LogException(e);
            return ServiceResultBuilder.BuildFailure<WorkerDto>(ServiceResultError.DatabaseError);
        }
    }

    private IEnumerable<WorkerDto> MapWorkersToDtos(IEnumerable<Worker> workers)
    {
        List<WorkerDto> result = [];
        result.AddRange(workers.Select(mapper.Map<WorkerDto>));
        return result;
    }

    private static Worker MapWorkerDtoToWorker(WorkerDto newWorkerDto, Worker worker)
    {
        worker.FirstName = newWorkerDto.FirstName;
        worker.LastName = newWorkerDto.LastName;
        worker.PhoneNumber = newWorkerDto.PhoneNumber;
        
        worker.Address = newWorkerDto.Address;
        worker.City = newWorkerDto.City;
        worker.State = newWorkerDto.State;
        
        worker.Description = newWorkerDto.Description;
        worker.MinimalSalary = newWorkerDto.MinimalSalary;
        
        return worker;
    }
}