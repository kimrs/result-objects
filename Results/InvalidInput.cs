namespace Results;

public class InvalidInput<T> : Result<T>
{
    public ValidationFailure[] Failures { get; }

    internal InvalidInput(ValidationFailure[] failures)
    {
        Failures = failures;
    }

    public static implicit operator InvalidInput<T>(ValidationFailure failure)
        => new([failure]);

    public static implicit operator InvalidInput<T>(ValidationFailure[] failures)
        => new(failures);
}

public abstract partial class Result<T>
{
    public static implicit operator Result<T>(ValidationFailure failure)
        => (InvalidInput<T>) failure;
    
    public static implicit operator Result<T>(ValidationFailure[] failures)
        => (InvalidInput<T>) failures;
}