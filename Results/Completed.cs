namespace Results;

public class Completed<T> : Result<T>
{
	public T Value { get; }

	internal Completed(T value)
	{
		Value = value;
	}

	public static implicit operator Completed<T>(T value)
		=> new (value);
}

public abstract partial class Result<T>
{
	public static implicit operator Result<T>(T value)
		=> (Completed<T>)  value;
}