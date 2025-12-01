namespace Ergasia_API.Data.Enums;

public enum ServiceResultError
{
    NoError,
    NotFound,
    EmptyCollection,
    DependenciesNotFound,
    InvalidArgument,
    InvalidCredentials,
    DuplicitId,
    UnableToChange,
    TokenError,
    DatabaseError,
}