namespace Results;

public static partial class Extensions
{
    private static Result<TR> _then<T, TR>(
        this Result<T> result,
        Func<T, Result<TR>> f
    ) => result switch
    {
        Completed<T> c => f(c.Value),
        Exceptional<T> e => e.Exception,
        _ => new ArgumentOutOfRangeException(nameof(result))
    };
    
    public static Result<TR> Then<T, TR>(
        this Result<T> result,
        Func<T, Result<TR>> f
    ) => result._then(f);

    public static Result<TR> Then<T, TR>(
        this Result<T> result,
        Func<T, TR> f
    ) => result._then(x => new Completed<TR>(f(x)));
    
    public static async Task<Result<TR>> Then<T, TR>(
        this Result<T> result,
        Func<T, Task<Result<TR>>> f
    ) => result switch
    {
        Completed<T> c => await f(c.Value),
        Exceptional<T> e => e.Exception,
        _ => new ArgumentOutOfRangeException(nameof(result))
    };

    public static async Task<Result<TR>> Then<T, TR>(
        this Task<Result<T>> t,
        Func<T, Task<Result<TR>>> f)
    {
        var r = await t;
        return await r.Then(f);
    }

    public static async Task<Result<TR>> Then<T, TR>(
        this Task<Result<T>> t,
        Func<T, Result<TR>> f)
    {
        var r = await t;
        return r.Then(f);
    }
}