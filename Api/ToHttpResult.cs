using Domain;
using Results;

namespace Api;

public static class Extensions
{
    public static async Task<IResult> ToHttpResult<T>(this Task<Result<T>> task, Func<T, IResult> onCompleted)
    {
        var result = await task;
        return result switch
        {
            Completed<T> c => onCompleted(c.Value),
            Exceptional<T> { Exception: DomainException e } => Microsoft.AspNetCore.Http.Results.BadRequest(e.Message),
            _ => Microsoft.AspNetCore.Http.Results.InternalServerError()
        };
    }
}