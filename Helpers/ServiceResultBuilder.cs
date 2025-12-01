using Ergasia_API.Data;
using Ergasia_API.Data.Enums;

namespace Ergasia_API.Helpers;

public static class ServiceResultBuilder
{
    public static ServiceResult<T> BuildSuccess<T>(T? data)
    {
        return new ServiceResult<T>
        {
            Data = data,
            IsSuccess = true,
            Error = default
        };
    }
    
    public static ServiceResult<T> BuildSuccess<T>()
    {
        return new ServiceResult<T>
        {
            Data = default,
            IsSuccess = true,
            Error = default
        };
    }
    
    public static ServiceResult<T> BuildFailure<T>(ServiceResultError error)
    {
        return new ServiceResult<T>
        {
            Data = default,
            IsSuccess = false,
            Error = error
        };
    }
}