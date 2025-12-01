using Ergasia_API.Data.Enums;

namespace Ergasia_API.Data;

public class ServiceResult<T>
{
    public T? Data { get; init; }
    public ServiceResultError Error { get; init; }
    
    public bool IsSuccess { get; init; }
}