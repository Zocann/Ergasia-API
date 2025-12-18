using Ergasia_API.Data.Enums;

namespace Ergasia_API.Helpers;

public static class GetStatusCode
{
    public static int BasedOnError(ServiceResultError error)
    {
        switch (error)
        {
            case ServiceResultError.DatabaseError:
            case ServiceResultError.TokenError:
                return 500;

            case ServiceResultError.EmptyCollection:
            case ServiceResultError.NotFound:
            case ServiceResultError.DependenciesNotFound:
                return 404;

            case ServiceResultError.DuplicitId:
            case ServiceResultError.InvalidArgument:
                return 400;

            case ServiceResultError.UnableToChange:
                return 403;

            case ServiceResultError.InvalidCredentials:
                return 401;

            case ServiceResultError.NoError:
            default:
                return 200;
        }
    }
}