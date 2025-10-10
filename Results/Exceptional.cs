namespace Results;

public class Exceptional<T> : Result<T>
{
    public Exception Exception { get; }

    private Exceptional(Exception exception)
    {
        Exception = exception;
    }

	public static implicit operator Exceptional<T>(
		Exception exception)
		=> new(exception);
}

public abstract partial class Result<T>
{
	public static implicit operator Result<T>(
		Exception exception)
		=> (Exceptional<T>) exception;
}