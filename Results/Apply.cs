namespace Results;

public static partial class Extensions
{
    public static Result<Func<T1, Result<Func<T2, Result<TR>>>>> Curry<T1, T2, TR>(
        this Func<T1, T2, Result<TR>> f
    ) => new Completed<Func<T1, Result<Func<T2, Result<TR>>>>>(t1 =>
        new Completed<Func<T2, Result<TR>>>(t2 => f(t1, t2)));

    public static Result<TR> Apply<T, TR>(
        this Result<Func<T, Result<TR>>> rf,
        Result<T> rx
    ) => rf.Then(f => rx.Then(x => f(x)));
    
}