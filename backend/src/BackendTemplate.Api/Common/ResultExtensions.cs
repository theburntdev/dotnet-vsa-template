using BackendTemplate.Domain.Common;

namespace BackendTemplate.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T, TResponse>(
        this Result<T> result,
        Func<T, TResponse> mapper,
        Func<TResponse, string>? location = null)
    {
        if (result.IsFailure)
            return ToProblem(result.Error, result.Kind);

        var response = mapper(result.Value);
        return location is not null
            ? TypedResults.Created(location(response), response)
            : TypedResults.Ok(response);
    }

    public static IResult ToHttpResult(this Result<Unit> result)
        => result.IsFailure
            ? ToProblem(result.Error, result.Kind)
            : TypedResults.NoContent();

    private static IResult ToProblem(string error, ErrorKind kind)
    {
        var (status, title) = kind switch
        {
            ErrorKind.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorKind.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status422UnprocessableEntity, "Validation Error")
        };

        return TypedResults.Problem(
            detail: error,
            statusCode: status,
            title: title,
            type: "https://tools.ietf.org/html/rfc7807");
    }
}
