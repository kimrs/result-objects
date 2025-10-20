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
            InvalidInput<T> vf => Microsoft.AspNetCore.Http.Results.BadRequest(vf.Failures),
            _ => Microsoft.AspNetCore.Http.Results.InternalServerError()
        };
    }
}