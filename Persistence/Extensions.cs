using Results;

namespace Persistence;

internal static class Extensions
{
    public static Task<Result<T>> ToTask<T>(this T value)
        => Task.FromResult<Result<T>>(value);
}